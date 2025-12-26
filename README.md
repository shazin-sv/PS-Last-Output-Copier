# 📌 PS Last Output Copier
> **Stop manual highlighting. [cite_start]Capture your terminal context instantly.** [cite: 1]

[cite_start]**PS Last Output Copier** is a lightweight shell utility that captures your last executed command and its corresponding output (including errors), allowing you to copy both to the clipboard together instantly. [cite: 1] [cite_start]It is designed to streamline debugging, error sharing, and documentation without changing your normal terminal workflow. [cite: 1]

---

### ✨ Features
* [cite_start]**Complete Capture:** Grabs the last command entered plus the full output (stdout + stderr). [cite: 1]
* [cite_start]**Clean Formatting:** Copies data in a structured "COMMAND: / OUTPUT:" format for easy reading. [cite: 1]
* [cite_start]**Native Hotkey:** Uses a simple keybinding to trigger the copy action. [cite: 1]
* [cite_start]**Privacy First:** Works fully offline with no AI, internet, or APIs required. [cite: 1]
* [cite_start]**Lightweight:** Fast performance that keeps terminal behavior unchanged. [cite: 1]

---

### 🖥 Installation

#### **Windows**
1. [cite_start]Run the **`PSLastOutputSetup.exe`** installer. [cite: 1]
2. [cite_start]Follow the Setup Wizard: Accept the Terms and Conditions and click **Install**. [cite: 1]
3. [cite_start]**Restart** any open PowerShell windows to activate the utility. [cite: 1]

#### **Linux (PowerShell Core)**
1. [cite_start]Ensure you have **PowerShell Core (pwsh)** and a clipboard manager (**`xclip`** or **`wl-copy`** for Wayland) installed. [cite: 1, 9]
2. [cite_start]Copy the `Src` and `Linux` folders to your machine. [cite: 1, 10]
3. [cite_start]Open a terminal in the `Linux` folder and run: [cite: 1, 10]
   ```bash
   chmod +x install.sh
   ./install.sh
   [cite_start]``` [cite: 1, 11]
4. [cite_start]**Restart** `pwsh`. [cite: 1, 11]
   * [cite_start]*Note: The Ctrl+Shift+C binding works if your terminal supports passing those keys.* [cite: 1, 12]

---

### 🚀 How to Use
1. [cite_start]Use PowerShell or your terminal as you normally would. [cite: 2]
2. [cite_start]When you want to capture the last command and its results, press: [cite: 3]
   > [cite_start]**`[Ctrl] + [Shift] + [C]`** [cite: 3]
3. [cite_start]The content is now in your clipboard. [cite: 3]
4. [cite_start]**Paste** it anywhere (Slack, GitHub, Documentation, etc.). [cite: 4]

---

### 📋 Example Output
[cite_start]When pasted, your content will look like this: [cite: 1]

```text
COMMAND:
npm run build

OUTPUT:
Error: Module not found: payment_provider
[cite_start]``` [cite: 1]

---

### 🛠 Uninstallation
[cite_start]To remove the utility on Windows, run: [cite: 1]
[cite_start]`.\PSLastOutputSetup.exe /uninstall` [cite: 4]

[cite_start]*(Alternatively, you can manually delete the module folder)*. [cite: 4]