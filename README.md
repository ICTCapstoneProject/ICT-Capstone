# -ICT-Capstone

### Prerequisites
- **Visual Studio Code** must already be installed. Get a copy via: https://code.visualstudio.com/download

### Steps to Set Up the Project

1. **First, clone the repository using Visual Studio Code**:
    - Go to our Github repository. 
    - Press `Code`, then in the dropdown, ensuring `Https` is selected, copy the url.
   - Open **Visual Studio Code**.
   - In Visual Studio Code, using your keyboard, press `Cmd+Shift+P` to open the command palette.
   - In the field that pops up, type `Git: Clone `.
   - Seperated from `Clone` by a space, paste the URL you copied earlier and using your keyboard, press `return`. 
   - Choose the folder where you'd like to clone the repo.

2. **Open the cloned repository**:
   - After cloning the repository, you’ll be prompted to open the project folder. Press on `Open`.

**Install dev dependencies**: (Windows)
3. - https://dotnet.microsoft.com/en-us/download/dotnet/7.0 Install

4. - https://git-scm.com/downloads/win Install

5. - https://nodejs.org/en/download/ Install

3. **Open the terminal in Visual Studio Code**: (Mac)
   - In Visual Studio Code, open the terminal by pressing `Terminal`, then in the dropdown, press `New Terminal`.

4. **Make the setup script executable**:
   In the terminal, type :
   `chmod +x setup.sh`
   Then, using your keyboard, press `return/enter`.

5. **Run the script**
   While still in the terminal type:
   `./setup.sh`
   Then, using your keyboard, press `return/enter`.

**All dependencies should now automatically attempt to install. Let this run to completion.**

6. **Run The project & Optionally Close it.**
   Now that all dependencies should be installed, run the project by:
   Inside the terminal, typing:
   `dotnet watch run`
   and using your keyboard, press `return/enter`.
   
   The project should now automatically open in chrome.

   **If you want to close the project**, simply, while in the terminal window, on your keyboard, press `control+c`.

   **To run it again, all you need to do in the future is** type `dotnet watch run` in a terminal window and on your keyboard, press `return/enter`.

**FOR DEV DEPLOYMENT** 

**(First time, only do this once) While running the project in vs code, within the project’s directory** type `cd electron` and hit `return/enter` then type `npm install electron --save-dev` and hit `enter` to install the deployment tools (electron)

**To make a new Mac build (Anytime)** type 
`npm run dist -- --mac` and hit `return/enter` letting the build creation run to completion. The build will be located within `TheProject’sDirectory/electron/dist` both as a dmg (to install to “applications”) and as a program in `mac-arm64` 

**To make a new Windows build (Anytime)** type
`npm run dist -- --win` and hit `return/enter` letting the build run to completion. The build will be located within `TheProject’sDirectory/electron/dist` both as a setup file (to install to programs) and as an exe in win-arm64-unpacked.


**NOTE: The server that the build points to must be running for the application to work.** If the project is being ran locally, open vscode, and if you are still within the electron dir (which you should be upon following these steps), type
`cd ..` to go back to the project's directory and hit `return/enter` then type 
`dotnet run` and hit `return/enter`. With the server it points to running, the application should now run.
