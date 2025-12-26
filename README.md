# 📌 PS Last Output Copier
> **Stop manual highlighting. Capture your terminal context instantly.** [cite: 1]

**PS Last Output Copier** is a lightweight shell utility that captures your last executed command and its corresponding output (including errors), allowing you to copy both to the clipboard together instantly. [cite: 1] It is designed to streamline debugging, error sharing, and documentation without changing your normal terminal workflow. [cite: 1]

---

### ✨ Features
* **Complete Capture:** Grabs the last command entered plus the full output (stdout + stderr). [cite: 1]
* **Clean Formatting:** Copies data in a structured "COMMAND: / OUTPUT:" format for easy reading. [cite: 1]
* **Native Hotkey:** Uses a simple keybinding to trigger the copy action. [cite: 1]
* **Privacy First:** Works fully offline with no AI, internet, or APIs required. [cite: 1]
* **Lightweight:** Fast performance that keeps terminal behavior unchanged. [cite: 1]

---

### 🖥 Installation

#### **Windows**
1. Run the **`PSLastOutputSetup.exe`** installer. [cite: 1]
2. Follow the Setup Wizard: Accept the Terms and Conditions and click **Install**. [cite: 1]
3. **Restart** any open PowerShell windows to activate the utility. [cite: 1]

#### **Linux (PowerShell Core)**
1. Ensure you have **PowerShell Core (pwsh)** and a clipboard manager (**`xclip`** or **`wl-copy`** for Wayland) installed. [cite: 1, 9]
2. Copy the `Src` and `Linux` folders to your machine. [cite: 1, 10]
3. Open a terminal in the `Linux` folder and run: [cite: 1, 10]
   ```bash
   chmod +x install.sh
   ./install.sh
   ``` [cite: 1, 11]
4. **Restart** `pwsh`. [cite: 1, 11]
   * *Note: The Ctrl+Shift+C binding works if your terminal supports passing those keys.* [cite: 1, 12]

---

### 🚀 How to Use
1. Use PowerShell or your terminal as you normally would. [cite: 2]
2. When you want to capture the last command and its results, press: [cite: 3]
   > **`[Ctrl] + [Shift] + [C]`** [cite: 3]
3. The content is now in your clipboard. [cite: 3]
4. **Paste** it anywhere (Slack, GitHub, Documentation, etc.). [cite: 4]

---

### 📋 Example Output
When pasted, your content will look like this: [cite: 1]

```text
COMMAND:
npm run build

OUTPUT:
Error: Module not found: payment_provider
``` [cite: 1]

---

### 🛠 Uninstallation
To remove the utility on Windows, run: [cite: 1]
`.\PSLastOutputSetup.exe /uninstall` [cite: 4]

*(Alternatively, you can manually delete the module folder)*. [cite: 4]