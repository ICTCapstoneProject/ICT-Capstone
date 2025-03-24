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

3. **Open the terminal in Visual Studio Code**:
   - In Visual Studio Code, open the terminal by pressing `Terminal`, then in the dropdown, press `New Terminal`.

4. **Make the setup script executable**:
   In the terminal, type :
   `chmod +x setup.sh`
   Then, using your keyboard, press `return`.

5. **Run the script**
   While still in the terminal type:
   `./setup.sh`
   Then, using your keyboard, press `return`.

**All dependencies should now automatically attempt to install. Let this run to completion.**

6. **Run The project & Optionally Close it.**
   Now that all dependencies should be installed, run the project by:
   Inside the terminal, typing:
   `dotnet watch run`
   and using your keyboard, press `return`.
   
   The project should now automatically open in chrome.

   **If you want to close the project**, simply, while in the terminal window, on your keyboard, press `control+c`.

   **To run it again, all you need to do in the future is** type `dotnet watch run` in a terminal window and on your keyboard, press `return`.

