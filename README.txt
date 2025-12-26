# 📌 PS Last Output Copier

> **Stop manual highlighting. Capture your terminal context instantly.**

**PS Last Output Copier** is a lightweight shell utility that captures your last executed command and its corresponding output (including errors), allowing you to copy both to the clipboard together instantly. It is designed to streamline debugging, error sharing, and documentation without changing your normal terminal workflow.

---

### ✨ Features

* 
**Complete Capture:** Grabs the last command entered plus the full output (stdout + stderr).


* 
**Clean Formatting:** Copies data in a structured "COMMAND: / OUTPUT:" format for easy reading.


* 
**Native Hotkey:** Uses a simple keybinding to trigger the copy action.


* 
**Privacy First:** Works fully offline with no AI, internet, or APIs required.


* 
**Lightweight:** Fast performance that keeps terminal behavior unchanged.



---

### 🖥 Installation

#### **Windows**

1. Run the **`PSLastOutputSetup.exe`** installer.


2. Follow the Setup Wizard: Accept the Terms and Conditions and click **Install**.


3. 
**Restart** any open PowerShell windows to activate the utility.



#### **Linux (PowerShell Core)**

1. Ensure you have **PowerShell Core (pwsh)** and a clipboard manager (**`xclip`** or **`wl-copy`** for Wayland) installed.


2. Copy the `Src` and `Linux` folders to your machine.


3. Open a terminal in the `Linux` folder and run:


```bash
chmod +x install.sh
./install.sh

```


4. 
**Restart** `pwsh`.



---

### 🚀 How to Use

1. Use your PowerShell/terminal as you normally would.


2. When you want to capture the last command and its results, press:
> 
> **`[Ctrl] + [Shift] + [C]`** 
> 
> 


3. The content is now in your clipboard.


4. 
**Paste** it anywhere (Slack, GitHub, Documentation, etc.).



---

### 📋 Example Output

When pasted, your content will look like this:

```text
COMMAND:
npm run build

OUTPUT:
Error: Module not found: payment_provider

```

---

### 🛠 Uninstallation

To remove the utility on Windows, run:
`.\PSLastOutputSetup.exe /uninstall`


*(Alternatively, you can manually delete the module folder)*.