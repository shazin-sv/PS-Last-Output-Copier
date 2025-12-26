
<#
.Module Name
    PSLastOutput
.Synopsis
    Captures the last command and its output for easy clipboard copying.
.Description
    Automatically starts a transcription session to capture console output.
    Provides a command and shortcut (Ctrl+Shift+C) to copy the last command 
    and its output to the clipboard.
#>

$script:currentTranscriptPath = $null
$script:offset = 0
$script:lastRangeStart = 0
$script:lastRangeEnd = 0
$script:originalPrompt = $null
$script:hooked = $false

function Start-PSLOTranscript {
    $tempDir = [System.IO.Path]::GetTempPath()
    $guid = [Guid]::NewGuid().ToString()
    $script:currentTranscriptPath = Join-Path $tempDir "PSLastOutput_$guid.txt"

    # Cleanup old transcripts (older than 3 days)
    Get-ChildItem $tempDir -Filter "PSLastOutput_*.txt" -File | 
        Where-Object { $_.LastWriteTime -lt (Get-Date).AddDays(-3) } | 
        Remove-Item -Force -ErrorAction SilentlyContinue

    # Start Transcript
    # We use -Force to override if one exists, but we generated a GUID so improbable.
    # Note: This prints "Transcript started..." which we cannot easily hide without hacks.
    try {
        Start-Transcript -Path $script:currentTranscriptPath -Append -Force -ErrorAction Stop
        
        # Initialize offset to current size (skips header)
        if (Test-Path $script:currentTranscriptPath) {
             $script:offset = (Get-Item $script:currentTranscriptPath).Length
        }
    } catch {
        Write-Warning "PSLastOutput: Unable to start transcript. Capture features disabled."
    }
}

function Copy-LastCommand {
    <#
    .Synopsis
        Copies the last executed command and its output to the clipboard.
    #>
    [CmdletBinding()]
    param()

    if (-not $script:currentTranscriptPath -or -not (Test-Path $script:currentTranscriptPath)) {
        Write-Warning "No active transcript found."
        return
    }

    try {
        # Use a FileStream to read with FileShare.ReadWrite to avoid locking issues
        $stream = [System.IO.File]::Open($script:currentTranscriptPath, [System.IO.FileMode]::Open, [System.IO.FileAccess]::Read, [System.IO.FileShare]::ReadWrite)
        $reader = New-Object System.IO.StreamReader($stream)

        $start = $script:lastRangeStart
        $end = $script:lastRangeEnd
        $len = $end - $start
        
        if ($len -le 0) {
            Write-Warning "No output captured for the last command."
            return
        }

        # Validate bounds
        if ($start -lt 0 -or $end -gt $stream.Length) {
             # Fallback: Read last 2KB if bounds are wonky
             $start = [Math]::Max(0, $stream.Length - 2048)
             $len = $stream.Length - $start
        }

        $stream.Position = $start
        $buffer = New-Object char[] $len
        $readBytes = $reader.Read($buffer, 0, $len)
        $rawOutput = [string]::new($buffer, 0, $readBytes)

        # Get Command from History
        $lastHist = Get-History -Count 1
        $cmdText = if ($lastHist) { $lastHist.CommandLine } else { "Unknown Command" }

        # Clean up output
        # Remove transcript headers if they appear (unlikely in short block)
        # We might want to trim leading/trailing newlines
        $cleanOutput = $rawOutput.Trim()

        # Remove the echoed command if it appears at the start (Transcript does this)
        # But robust generic way: just put it in the "OUTPUT" block.
        # User requested readable format.

        $finalText = "COMMAND:`r`n$cmdText`r`n`r`nOUTPUT:`r`n$cleanOutput"
        
        if ($IsLinux) {
             # On Linux, Set-Clipboard often requires xsel/xclip/wl-copy.
             # We try Set-Clipboard first, if it fails or if we want to be robust:
             try {
                 Set-Clipboard -Value $finalText -ErrorAction Stop
             } catch {
                 # Fallback to direct tools
                 if (Get-Command "xclip" -ErrorAction SilentlyContinue) {
                     $finalText | xclip -selection clipboard
                 } elseif (Get-Command "wl-copy" -ErrorAction SilentlyContinue) {
                     $finalText | wl-copy
                 } else {
                     Write-Warning "Clipboard tool not found (install xclip or wl-copy)."
                 }
             }
        } else {
             Set-Clipboard -Value $finalText
        }
        Write-Host " [PSLastOutput] Last command & output copied to clipboard." -ForegroundColor Cyan

    } catch {
        Write-Error "Failed to copy output: $_"
    } finally {
        if ($reader) { $reader.Dispose() }
        if ($stream) { $stream.Dispose() }
    }
}

function Register-Hook {
    # We hook the global prompt
    # We save the *text* of the original prompt or the scriptblock
    
    # Check if we already hooked
    if ($script:hooked) { return }

    $origPrompt = Get-Content Function:\prompt -ErrorAction SilentlyContinue
    if (-not $origPrompt) {
        $origPrompt = { "PS $PWD> " }
    }
    
    $script:originalPrompt = $origPrompt

    # Define new prompt
    # We use Invoke-Expression or ScriptBlock creation to wrap it
    # But dealing with closures in Global scope is tricky.
    # We will redefine the function prompt in Global scope.
    
    $sb = [scriptblock]::Create(@"
        # PSLastOutput Hook
        if (Test-Path '$script:currentTranscriptPath') {
             try {
                `$cur = (Get-Item '$script:currentTranscriptPath').Length
                # Update module variables via module scope?
                # accessing module scope from global is hard.
                # We call a public exported function? Or use variables?
                # Better: Call a script-scope function using the module reference?
                # Or just put logic here if we can access the path (we hardcoded it in the string above).
                
                # To communicate back to module, we can use a global variable or import the function.
                # Let's export 'Update-LOState' and call it.
                Update-LOState -CurrentSize `$cur
             } catch {}
        }
        
        # Original Prompt
        $origPrompt
"@)
    
    Set-Item -Path Function:\global:prompt -Value $sb
    $script:hooked = $true
}

function Update-LOState {
    param($CurrentSize)
    $script:lastRangeStart = $script:offset
    $script:lastRangeEnd = $CurrentSize
    $script:offset = $CurrentSize
}

function Register-KeyHandler {
    if (Get-Command Set-PSReadLineKeyHandler -ErrorAction SilentlyContinue) {
        try {
            Set-PSReadLineKeyHandler -Chord 'Ctrl+Shift+c' -ScriptBlock {
                Copy-LastCommand
            }
        } catch {
            Write-Warning "PSLastOutput: Could not register Ctrl+Shift+C handler."
        }
    }
}

Export-ModuleMember -Function Copy-LastCommand, Update-LOState
# We do not export Update-LOState normally, but the prompt hook needs it. 
# If we define the prompt hook *inside* the module, it's fine, but 'prompt' must be global.
# When 'prompt' runs, it runs in global scope.
# So 'Update-LOState' must be available.

# Startup Logic
Start-PSLOTranscript
Register-Hook
Register-KeyHandler
