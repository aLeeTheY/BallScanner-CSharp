<!-- Improved compatibility of back to top link: See: https://github.com/othneildrew/Best-README-Template/pull/73 -->

<span id="readme-top"></span>

<!-- ! --- REPO SHIELDS --->
<!-- ! ---------------- --->

<div align="center">

[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![License][license-shield]][license-url]

![GitHub Last Commit][last-commit-shield]
![GitHub Repo Size][repo-size-shield]

[![English](https://img.shields.io/badge/English-indigo?style=for-the-badge)](README.md)
[![Русский](https://img.shields.io/badge/Русский-purple?style=for-the-badge)](README.ru.md)

</div>

<!-- ! --- REPO HEADER --->
<!-- ! --------------- --->

<br />
<div align="center">
  <h1 align="center">Ball Scanner [C#]</h1>

  <p align="center">
    🔬 WPF desktop app for optical quality control and grade determination of retroreflective glass beads used in road markings.
    <br />
    <br />
    <img src="docs/assets/00__repo_cover.png" alt="Repository cover" width="100%" />
    <hr />
    <b><a href="https://github.com/aLeeTheY/BallScanner-CSharp/releases/tag/1.1.0-beta">Download</a></b>
    &middot;
    <b><a href="https://github.com/aLeeTheY/BallScanner-CSharp/issues/new?labels=bug&template=bug-report.md">Report Bug</a></b>
    <hr />
  </p>
</div>

<!-- ! --- TABLE OF CONTENTS --->
<!-- ! --------------------- --->

<details>
  <summary>📦 Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">ℹ️ About the Project</a>
      <ul>
        <li>
          <a href="#showcase">🎬 Showcase</a>
          <ul>
            <li><a href="#workflow-showcase">🔄 Basic Usage Scenario</a></li>
            <li><a href="#multithreading-showcase">⚡ Multithreading Implementation</a></li>
            <li><a href="#binarization-showcase">🔬 Image Binarization</a></li>
            <li><a href="#gui-showcase">🖥️ GUI Overview</a></li>
          </ul>
        </li>
        <li><a href="#key-features">✨ Key Features</a></li>
        <li><a href="#built-with">🧰 Built With</a></li>
        <li><a href="#project-structure">📂 Project Structure</a></li>
      </ul>
    </li>
    <li>
      <a href="#quick-start">🚀 Quick Start</a>
      <ul>
        <li><a href="#prerequisites">📋 Prerequisites</a></li>
        <li><a href="#visual-studio-build">🅰️ Method 1: Building via Visual Studio</a></li>
        <li><a href="#cli-build">🅱️ Method 2: Building via Command Line</a></li>
        <li><a href="#nsis-build">💿 Building Installers via NSIS (Optional)</a></li>
      </ul>
    </li>
    <li><a href="#usage">💡 Usage</a></li>
    <li><a href="#development-challenges">🧠 Development Challenges</a></li>
    <li><a href="#key-skills">📈 Key Skills</a></li>
    <li><a href="#roadmap">🗺️ Roadmap</a></li>
    <li><a href="#license">📄 License</a></li>
    <li><a href="#contact">🤝 Contact</a></li>
    <li><a href="#acknowledgments">💖 Acknowledgments</a></li>
  </ol>
</details>

<!-- ! --- ABOUT THE PROJECT --->
<!-- ! --------------------- --->

## ℹ️ About the Project <span id="about-the-project"></span>

The goal of this project is to create a desktop application for optical quality control of retroreflective glass beads used in road markings. The application was developed on request from an enterprise and allows a factory employee to process a large number of microsphere images, determine the batch grade, and save inspection results to reports for further viewing.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- ! --- DEMO / SHOWCASE --->
<!-- ! ------------------- --->

### 🎬 Showcase <span id="showcase"></span>

Below is a video demonstration of the key capabilities of the program:

<div align="center">

#### 🔄 Basic Usage Scenario <span id="workflow-showcase"></span>

![Full workflow: from loading images to saving a report](docs/assets/01__usage_workflow.gif)

<p align="right">(<a href="#readme-top">back to top</a>)</p>

#### ⚡ Multithreading Implementation <span id="multithreading-showcase"></span>

![Loading all CPU cores to 100% while scanning many images](docs/assets/02__multithread.gif)

<p align="right">(<a href="#readme-top">back to top</a>)</p>

#### 🔬 Image Binarization <span id="binarization-showcase"></span>

![Adjusting the sensitivity threshold (Threshold) and setting threshold marks for glass bead grades](docs/assets/03__binarization.gif)

<p align="right">(<a href="#readme-top">back to top</a>)</p>

#### 🖥️ GUI Overview <span id="gui-showcase"></span>

![Navigation through the main pages of the application](docs/assets/04__gui.gif)

<p align="right">(<a href="#readme-top">back to top</a>)</p>

</div>

<!-- ! --- KEY FEATURES --->
<!-- ! ---------------- --->

### ✨ Key Features <span id="key-features"></span>

- 📊 **Accurate Quality Assessment:** automatic counting of defective (black) pixels in each loaded image, assigning a grade to it, and calculating the average value across the entire image batch.
- 🎚️ **Adjustable Binarization Threshold:** the operator manually sets the `Threshold` (0–255), compensating for differences in lighting and image quality — which is why the method works even on unevenly lit photos. The main requirement is that shooting conditions are approximately the same within a batch.
- ⚡ **Parallel Processing:** multithreaded analysis of images simultaneously (each CPU core takes a separate image) without freezing the UI — up to **3,000 images at 4320×3240 px in ~40 seconds** (~75 images/sec).
- 🗄️ **Data Management:** saving inspection results to a SQLite database (linked to the batch number and fraction), as well as convenient search and filtering by dates and other fields.
- 🔐 **Access Control:** protection of data from accidental modification or deletion through Administrator and Employee roles.
- 🎨 **Flexible Interface:** custom design, dark/light theme, localization (EN/RU), and responsive layout for different screen resolutions.
- 💻 **Broad Compatibility:** support for Windows 7 and above thanks to the .NET Framework 4.8 platform.
- 🪶 **Lightweight:** the installed program occupies only about 20 MB (excluding database data).
- 📦 **Easy Installation:** a portable version is available, as well as a classic PC installer.

> [!NOTE]
> **Benchmark Specs:**
>
> - **CPU:** AMD Ryzen 5 3600 (6C/12T)
> - **RAM:** 32 GB
> - **OS:** Windows 11 25H2
> - **Dataset:** 3,000 images (4320×3240 px)
> - **Result:** ~40 seconds (~75 images/sec)
>
> The image processing speed is **CPU-bound**: binarization and black-pixel counting are parallelized across CPU cores, so performance depends on core/thread count and will differ on other hardware.
>
> GPU acceleration (CUDA/OpenCL) may be implemented in the future — see the [Roadmap](#roadmap).

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- ! --- BUILT WITH --->
<!-- ! -------------- --->

### 🧰 Built With <span id="built-with"></span>

_For convenience, all information is divided into categories._

<details>
<summary>🧩 Architecture and Patterns</summary>

- [![MVVM][MVVM-logo]][MVVM-url] <sup>— architectural pattern for separating UI and logic (Model-View-ViewModel)</sup>
- [![MVVM Command Pattern][MVVMCommandPattern-logo]][MVVMCommandPattern-url] <sup>— behavioral pattern for encapsulating application actions into commands</sup>

</details>

<details>
<summary>🎨 Frontend (User Interface)</summary>

- [![WPF][WPF-logo]][WPF-url] <sup>— desktop UI technology (Windows Presentation Foundation)</sup>
- [![XAML][XAML-logo]][XAML-url] <sup>— declarative markup language for UI and styles</sup>

</details>

<details>
<summary>⚙️ Backend (Business Logic)</summary>

- [![C#][C#-logo]][C#-url] <sup>— main application development language</sup>
- [![.NET Framework 4.8][DNF48-logo]][DNF48-url] <sup>— application runtime platform</sup>
- [![Entity Framework][EntityFramework-logo]][EntityFramework-url] <sup>— ORM for working with the database through C# objects</sup>
- [![NLog][NLog-logo]][NLog-url] <sup>— library for logging application events</sup>

</details>

<details>
<summary>💾 Database</summary>

- [![SQLite][SQLite-logo]][SQLite-url] <sup>— lightweight embedded relational database</sup>

</details>

<details>
<summary>📦 Build and Installer Creation</summary>

- [![NSIS][NSIS-logo]][NSIS-url] <sup>— installer creation system for Windows (Nullsoft Scriptable Install System)</sup>

</details>

<details>
<summary>📝 Code Style and Formatting</summary>

- [![EditorConfig][EditorConfig-logo]][EditorConfig-url] <sup>— unified conventions for code formatting and style</sup>

</details>

<details>
<summary>🌿 Version Control and Repository Hosting</summary>

- [![Git][Git-logo]][Git-url] <sup>— distributed version control system</sup>
- [![GitHub][GitHub-logo]][GitHub-url] <sup>— platform for storing source code and publishing releases</sup>

</details>

<details>
<summary>🖥️ Development Tools</summary>

- [![Figma][Figma-logo]][Figma-url] <sup>— UI/UX mockup design tool</sup>
- [![Visual Studio][VisualStudio-logo]][VisualStudio-url] <sup>— primary IDE for C# and WPF application development</sup>
- [![Visual Studio Code][VSCode-logo]][VSCode-url] <sup>— auxiliary code editor</sup>
- [![SQLite DB Browser][SQLiteDBBrowser-logo]][SQLiteDBBrowser-url] <sup>— tool for viewing and editing SQLite databases</sup>

</details>

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- ! --- PROJECT STRUCTURE --->
<!-- ! --------------------- --->

### 📂 Project Structure <span id="project-structure"></span>

Main directories and files of the project:

```text
BallScanner-CSharp
│
├── .vscode/
│   └── settings.json                                   # VS Code workspace settings
│
├── aLeeTheY.Controls/                                  # library with custom UI components
│   ├── Properties/                                     # standard project files (build, resources, settings)
│   │   ├── AssemblyInfo.cs
│   │   ├── Resources.Designer.cs
│   │   ├── Resources.resx
│   │   ├── Settings.Designer.cs
│   │   └── Settings.settings
│   │
│   ├── Styles/                                         # styles for standard WPF components
│   │   ├── Button_Style.xaml
│   │   ├── ComboBox_Style.xaml
│   │   ├── GridSplitter_Style.xaml
│   │   ├── LabelTextBox_Inside_Style.xaml
│   │   ├── LabelTextBox_Multiline_Style.xaml
│   │   ├── PasswordBox_Style.xaml
│   │   ├── ProgressBar_Style.xaml
│   │   ├── ScrollBar_Style.xaml
│   │   ├── ScrollViewer_Style.xaml
│   │   ├── Slider_Style.xaml
│   │   ├── Styles.xaml
│   │   ├── TextBlock_Style.xaml
│   │   └── TextBox_Style.xaml
│   │
│   ├── Themes/                                         # styles for custom UI components
│   │   ├── BlockHeader_Style.xaml
│   │   ├── Generic.xaml
│   │   ├── JustButton_Style.xaml
│   │   ├── LabelComboBox_Style.xaml
│   │   ├── LabelTextBox_Style.xaml
│   │   ├── MenuButton_Style.xaml
│   │   └── SimpleRadioButton_Style.xaml
│   │
│   ├── aLeeTheY.Controls.csproj
│   ├── aLeeTheY__KeyFile.pfx                           # digital certificate file for signing (unofficial)
│   ├── BlockHeader.cs
│   ├── JustButton.cs
│   ├── LabelComboBox.cs
│   ├── LabelPasswordBox.cs
│   ├── LabelTextBox.cs
│   ├── MenuButton.cs
│   └── SimpleRadioButton.cs
│
├── aLeeTheY.Resources/                                 # third-party resources library
│   ├── Fonts/
│   │   ├── Montserrat/
│   │   └── Montserrat_Alternates/
│   │
│   ├── Properties/                                     # standard project files (build, resources, settings)
│   │   ├── AssemblyInfo.cs
│   │   ├── Resources.Designer.cs
│   │   ├── Resources.resx
│   │   ├── Settings.Designer.cs
│   │   └── Settings.settings
│   │
│   └── aLeeTheY.Resources.csproj
│
├── aLeeTheY.Utils/                                     # utilities and helper classes library
│   ├── Behaviors/                                      # attached behaviors for UI elements
│   │   └── ScaleFontBehavior.cs
│   │
│   ├── Converters/                                     # value converters
│   │   ├── BlockingConverter.cs
│   │   ├── LogicalConverter.cs
│   │   ├── NotePreviewConverter.cs
│   │   └── RowToIndexConverter.cs
│   │
│   ├── Helpers/                                        # helper classes
│   │   ├── Converters.cs
│   │   └── VisualHelper.cs
│   │
│   ├── Observers/                                      # observers (tracking size changes, etc.)
│   │   └── SizeObserver.cs
│   │
│   ├── Other/                                          # other helper classes
│   │   ├── SetMinWidthToAutoAttachedBehavior.cs
│   │   └── ViewBoxConstantFontSizeConverter.cs
│   │
│   ├── Properties/                                     # standard project files
│   │   └── AssemblyInfo.cs
│   │
│   ├── Services/                                       # services (authorization, hashing, window management)
│   │   ├── IAuthenticationService.cs
│   │   ├── SHAService.cs
│   │   └── WindowService.cs
│   │
│   ├── aLeeTheY.Utils.csproj
│   ├── aLeeTheY__KeyFile.pfx                           # digital certificate file for signing (unofficial)
│   └── packages.config                                 # list of NuGet packages
│
├── BallScanner/                                        # main project (application)
│   ├── Data/
│   │   ├── Tables/                                     # database entities (tables), accessible from any part of the application
│   │   │   ├── Report.cs
│   │   │   └── User.cs
│   │   │
│   │   ├── AppDbContext.cs                             # database connection context (Entity Framework)
│   │   └── db.sqlite3                                  # SQLite database file (template)
│   │
│   ├── MVVM/                                           # MVVM architecture (separation of data, logic, and UI)
│   │   ├── Base/                                       # base classes for view models
│   │   │   ├── BaseViewModel.cs
│   │   │   └── PageVM.cs
│   │   │
│   │   ├── Commands/                                   # commands (Command pattern implementation)
│   │   │   └── RelayCommand.cs
│   │   │
│   │   ├── Models/                                     # page-level data models (not DB tables)
│   │   │   └── Main/
│   │   │       ├── BallGrade.cs
│   │   │       └── ImageData.cs
│   │   │
│   │   ├── ViewModels/                                 # application logic for windows and pages
│   │   │   ├── Add/                                    # user registration window
│   │   │   │   ├── RegistrationVM.cs
│   │   │   │   └── RootVM.cs
│   │   │   │
│   │   │   ├── Auth/                                   # authorization window
│   │   │   │   ├── LoginVM.cs
│   │   │   │   └── RootVM.cs
│   │   │   │
│   │   │   ├── Edit/                                   # data editing windows
│   │   │   │   ├── EditReportsVM.cs
│   │   │   │   ├── EditUsersVM.cs
│   │   │   │   └── RootVM.cs
│   │   │   │
│   │   │   └── Main/                                   # main application window and its pages
│   │   │       ├── AboutVM.cs
│   │   │       ├── AccountManagementVM.cs
│   │   │       ├── AccountVM.cs
│   │   │       ├── CalibrateVM.cs
│   │   │       ├── DocumentsVM.cs
│   │   │       ├── MenuVM.cs
│   │   │       ├── RootVM.cs
│   │   │       ├── ScanVM.cs
│   │   │       └── SettingsVM.cs
│   │   │
│   │   └── Views/                                      # UI (markup) of windows and pages
│   │       ├── Add/                                    # user registration window
│   │       │   ├── RegistrationV.xaml
│   │       │   ├── RegistrationV.xaml.cs
│   │       │   ├── RootV.xaml
│   │       │   └── RootV.xaml.cs
│   │       │
│   │       ├── Auth/                                   # authorization window
│   │       │   ├── LoginV.xaml
│   │       │   ├── LoginV.xaml.cs
│   │       │   ├── RootV.xaml
│   │       │   └── RootV.xaml.cs
│   │       │
│   │       ├── Edit/                                   # data editing windows
│   │       │   ├── EditReportsV.xaml
│   │       │   ├── EditReportsV.xaml.cs
│   │       │   ├── EditUsersV.xaml
│   │       │   ├── EditUsersV.xaml.cs
│   │       │   ├── RootV.xaml
│   │       │   └── RootV.xaml.cs
│   │       │
│   │       └── Main/                                   # main application window and its pages
│   │           ├── AboutV.xaml
│   │           ├── AboutV.xaml.cs
│   │           ├── AccountManagementV.xaml
│   │           ├── AccountManagementV.xaml.cs
│   │           ├── AccountV.xaml
│   │           ├── AccountV.xaml.cs
│   │           ├── CalibrateV.xaml
│   │           ├── CalibrateV.xaml.cs
│   │           ├── DocumentsV.xaml
│   │           ├── DocumentsV.xaml.cs
│   │           ├── MenuV.xaml
│   │           ├── MenuV.xaml.cs
│   │           ├── RootV.xaml
│   │           ├── RootV.xaml.cs
│   │           ├── ScanV.xaml
│   │           ├── ScanV.xaml.cs
│   │           ├── SettingsV.xaml
│   │           └── SettingsV.xaml.cs
│   │
│   ├── Properties/                                     # standard project files (build, resources, settings)
│   │   ├── AssemblyInfo.cs
│   │   ├── Resources.Designer.cs
│   │   ├── Resources.resx
│   │   ├── Settings.Designer.cs
│   │   └── Settings.settings
│   │
│   ├── Resources/                                      # local project resources
│   │   ├── Fonts/                                      # fonts
│   │   │   ├── Montserrat/
│   │   │   ├── Montserrat_Alternates/
│   │   │   └── Fonts.xaml
│   │   │
│   │   ├── Icons/                                      # vector icons (XAML geometry)
│   │   │   └── Icons.xaml
│   │   │
│   │   ├── Languages/                                  # translation files (localization)
│   │   │   ├── Language.en-US.xaml
│   │   │   └── Language.ru-RU.xaml
│   │   │
│   │   ├── Palettes/                                   # palettes (color schemes) for pages
│   │   │   ├── Dark                                    # dark theme
│   │   │   │   ├── Blue.xaml
│   │   │   │   ├── Green.xaml
│   │   │   │   ├── Orange.xaml
│   │   │   │   ├── Pink.xaml
│   │   │   │   ├── Purple.xaml
│   │   │   │   ├── Red.xaml
│   │   │   │   └── Yellow.xaml
│   │   │   │
│   │   │   ├── Light                                   # light theme
│   │   │   │   ├── Blue.xaml
│   │   │   │   ├── Green.xaml
│   │   │   │   ├── Orange.xaml
│   │   │   │   ├── Pink.xaml
│   │   │   │   ├── Purple.xaml
│   │   │   │   ├── Red.xaml
│   │   │   │   └── Yellow.xaml
│   │   │   │
│   │   │   └── Old                                     # old variant (unused)
│   │   │       ├── Blue/
│   │   │       │   ├── Dark.xaml
│   │   │       │   └── Light.xaml
│   │   │       │
│   │   │       ├── Green/
│   │   │       │   ├── Dark.xaml
│   │   │       │   └── Light.xaml
│   │   │       │
│   │   │       ├── Orange/
│   │   │       │   ├── Dark.xaml
│   │   │       │   └── Light.xaml
│   │   │       │
│   │   │       ├── Purple/
│   │   │       │   ├── Dark.xaml
│   │   │       │   └── Light.xaml
│   │   │       │
│   │   │       ├── Red/
│   │   │       │   ├── Dark.xaml
│   │   │       │   └── Light.xaml
│   │   │       │
│   │   │       ├── Yellow/
│   │   │       │   ├── Dark.xaml
│   │   │       │   └── Light.xaml
│   │   │       │
│   │   │       ├── Dark.xaml
│   │   │       └── Light.xaml
│   │   │
│   │   ├── Styles/                                     # styles for local components not related to MVVM pages
│   │   │   ├── CustomWindowStyle.cs
│   │   │   ├── CustomWindowStyle.xaml                  # window styling
│   │   │   ├── DataGrid_Style.cs
│   │   │   ├── DataGrid_Style.xaml                     # table styling
│   │   │   └── WindowChromeButtons.xaml                # window control buttons (minimize, maximize, close)
│   │   │
│   │   └── Values/                                     # global constants and values (colors, sizes, strings)
│   │       └── Values.xaml
│   │
│   ├── aLeeTheY__KeyFile.pfx                           # digital certificate file for signing (unofficial)
│   ├── App.config                                      # .NET application configuration
│   ├── app.manifest                                    # application manifest (permissions, DPI, Windows version)
│   ├── App.xaml                                        # application entry point
│   ├── App.xaml.cs
│   ├── BallScanner.csproj
│   ├── favicon.ico                                     # application icon
│   ├── NLog.config                                     # logging configuration (NLog)
│   ├── NLog.xsd
│   └── packages.config                                 # list of NuGet packages
│
├── docs/                                               # documentation and resources for README
│   └── assets/
│       ├── ffmpeg/                                     # commands for converting .mp4 --> .gif
│       │   └── commands.txt
│       │
│       ├── 00__repo_cover.png                          # repository cover
│       ├── 01__usage_workflow.gif
│       ├── 02__multithread.gif
│       ├── 03__binarization.gif
│       ├── 04__gui.gif
│       ├── quick_start__1.png
│       └── quick_start__2.png
│
├── installer/                                          # source files for building the installer (NSIS)
│   ├── external/                                       # third-party installers used in NSIS scripts
│   │   ├── ndp48-web.exe                               # .NET Framework 4.8 web installer
│   │   └── signtool.exe                                # file signing utility (used in sign.bat)
│   │
│   ├── output/                                         # folder for ready installers
│   │   └── .gitkeep
│   │
│   ├── BallScanner_windows_x64_setup.nsi               # installer build script for 64-bit version
│   ├── BallScanner_windows_x86_setup.nsi               # installer build script for 32-bit version
│   └── sign.bat                                        # installer signing script with digital certificate
│
├── samples/                                            # test images for verifying operation
│   ├── defect/                                         # defective glass beads
│   ├── good/                                           # 1st grade glass beads
│   └── normal/                                         # 2nd grade glass beads
│
├── .editorconfig                                       # code formatting settings
├── .gitattributes
├── .gitignore
├── BallScanner-CSharp.sln                              # Visual Studio solution file (combines all projects)
├── LICENSE                                             # PolyForm Noncommercial License 1.0.0 license file
├── README.md
└── README.ru.md
```

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- ! --- QUICK START --->
<!-- ! --------------- --->

## 🚀 Quick Start <span id="quick-start"></span>

_Follow the instructions below to build and run the program on your PC._

### 📋 Prerequisites <a id="prerequisites"></a>

1. To build and run the project, you will need **Visual Studio Community 2026**. You can download it from the [official Microsoft website][VisualStudioInstaller-url].<br />
   During installation, in the **Workloads** tab, be sure to select: **`.NET desktop development`** (.NET desktop application development).

> [!TIP]
> Example of correct **`.NET desktop development`** workload installation in Visual Studio Installer:
>
> ![Screenshot of the required workload in Visual Studio Installer](docs/assets/quick_start__1.png)

2. Download this repository as a ZIP archive or clone it using [Git][Git-url] and navigate to the project directory:

```powershell
git clone https://github.com/aLeeTheY/BallScanner-CSharp
cd BallScanner-CSharp/
```

Next, the build process can be performed in two ways. Choose the one that suits you.

---

> [!WARNING]
> The methods below describe building the Portable version of the application. Depending on the chosen build platform, the files will be located in one of the following directories:
>
> ```powershell
> # regular build (without specifying /p:Platform)
> BallScanner/bin/Release/
>
> # build for x64 systems (flag /p:Platform=x64)
> BallScanner/bin/x64/Release/
>
> # build for x86 systems (flag /p:Platform=x86)
> BallScanner/bin/x86/Release/
> ```

---

### 🅰️ Method 1: Building via Visual Studio <a id="visual-studio-build"></a>

_This is the simplest method, requiring no command-line knowledge._

1. Launch Visual Studio.
2. In the program window, open the solution file **`BallScanner-CSharp.sln`**.
3. After the project initializes, in the top menu select **Build -> Build Solution** (or press `Ctrl+Shift+B`).
4. When the build completes, click the **Start** button on the top toolbar (or press `F5`).
5. The application is running.

---

### 🅱️ Method 2: Building via Command Line <a id="cli-build"></a>

_For those who prefer to work via terminal._

1. In addition to the already installed Visual Studio, you need to [**download NuGet CLI**][NuGetCLI]. After downloading, add `nuget.exe` to your OS `PATH` environment variables or place this file in the current directory next to the **`BallScanner-CSharp.sln`** solution.
2. Visual Studio provides a command prompt with a configured environment: `x64 Native Tools Command Prompt for VS`. Launch it.

> [!TIP]
> Demonstration of the launched terminal with the development environment `x64 Native Tools Command Prompt for VS`:
>
> ![x64 Native Tools Command Prompt for VS](docs/assets/quick_start__2.png)

3. In the opened terminal, run three commands to build and launch the project:

```powershell
# 1. Restore NuGet dependencies (Entity Framework, NLog, SQLite)
nuget restore BallScanner-CSharp.sln

# 2. Build the solution in Release configuration for 64-bit systems
#    (to build the 32-bit version, use the /p:Platform=x86 flag)
msbuild BallScanner-CSharp.sln /p:Configuration=Release /p:Platform=x64 /m

# 3. Launch the built application (quotes are required due to the space)
.\BallScanner\bin\x64\Release\"Ball Scanner.exe"
```

4. The application is running.

---

### 💿 Building Installers via NSIS (Optional) <a id="nsis-build"></a>

_Using the build methods described above, you can obtain Portable versions of the program for x86 and x64 systems. This instruction describes how to create an installer from these Portable versions._

1. Download and install the [**Nullsoft Scriptable Install System (NSIS)**][NSISInstaller] compiler.
2. Open a terminal and make sure the `makensis.exe` command is available; otherwise, add it to your OS `PATH` environment variables.
3. Also, make sure you have built the Portable version with a specific system bitness (`x86` or `x64`).
4. From the project root, navigate to the `installer/` directory and run the list of commands below to obtain ready installers:

```powershell
# navigate to the BallScanner/installer/ directory
cd installer/

# build installer for x64 version
makensis.exe BallScanner_windows_x64_setup.nsi

# build installer for x86 version
makensis.exe BallScanner_windows_x86_setup.nsi

# digital signing of installers
.\sign.bat
```

The built installers are located in the `BallScanner/installer/output/` folder.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- ! --- USAGE --->
<!-- ! --------- --->

## 💡 Usage <span id="usage"></span>

After launching the application, an authorization window will open. Use the following credentials to log into the demo version:

```text
Login:    admin
Password: admin
```

After authorization, the main program interface will become available to you. For a visual demonstration of the application, see the [**Showcase**](#showcase) section.

> [!TIP]
> For a quick check of the application, it is recommended to use microsphere photos from the `samples/` directory. They are categorized by quality: `good` — 1st grade, `normal` — 2nd grade, `defect` — reject.

> [!IMPORTANT]
> **Before the first scan, the scanner must be calibrated and grade thresholds set:**
>
> 1. On the **Calibration** page, adjust the `Threshold` slider (0–255) to match the current lighting.
> 2. Load the **1st-grade** image batch. The program will calculate the black pixel count for each image.
> 3. Find the maximum value across the batch and **manually enter** it into the **First grade** field.
> 4. Repeat steps 2–3 for the **2nd-grade** samples and fill in the **Second grade** field.
>
> _After that, the program will automatically assign grades when checking any other batches based on the configured scanner settings._

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- ! --- DEVELOPMENT CHALLENGES --->
<!-- ! -------------------------- --->

## 🧠 Development Challenges <span id="development-challenges"></span>

- **Creating a custom design system:** WPF lacks many ready-made components that match the design, so for some of them `ControlTemplate`s had to be implemented manually to make the entire visual work in synergy.
- **Mastering MVVM:** the transition from the event-driven model (`code-behind`) to data binding (`Data Binding`) and triggers required time to rethink the architecture.
- **Window customization (`WindowChrome`):** removing the standard Windows frame breaks the built-in window behavior (dragging, resizing), which is why this logic had to be implemented manually to satisfy the design.
- **Multithreading:** during image processing, the UI would freeze completely. The problem was solved by parallelization: the black pixel counting process is moved to a separate thread, where each CPU core processes a separate image, while the UI thread remains free.
- **Image processing:** studying methods for creating binary images via `Threshold` required reading documentation and applying trial and error.
- **Database queries:** working with Entity Framework and LINQ required understanding how to properly build queries against class objects without directly using SQL.
- **Fault tolerance:** users can make mistakes when working with the program. It was necessary to implement data validation and crash protection mechanisms (using `try/catch`) in all critical nodes of the program.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- ! --- KEY SKILLS --->
<!-- ! -------------- --->

## 📈 Key Skills <span id="key-skills"></span>

- **Development in the WPF ecosystem:** C#, XAML, .NET Framework 4.8, and NuGet.
- **Architecture:** designing applications using the **MVVM** pattern (through `Command` and `Data Binding` implementation).
- **UI/UX and design:** designing interfaces in **Figma** and implementing them using WPF tools.
- **Data handling:** designing the database structure, working with **SQLite**, the **SQLite DB Browser** utility, **Entity Framework**, and **LINQ** queries.
- **Access control:** implementing a role model (Administrator/Employee) with restricted visibility of certain windows/pages, as well as restrictions on modifying/deleting data in the database.
- **Installer creation:** writing **NSIS** scripts to compile installation files (installers) for Windows.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- ! --- ROADMAP --->
<!-- ! ----------- --->

## 🗺️ Roadmap <span id="roadmap"></span>

- [x] Designing UI/UX for application windows and pages in Figma
- [x] Designing the database structure and creating its template based on SQLite
- [x] Implementing the **MVVM** architectural pattern (Model, View, ViewModel) and configuring the database connection
- [x] Laying out the interface in XAML: implementing custom UI components and assembling them into a unified GUI
- [x] Developing the basic black dot counting algorithm (single-threaded loop)
- [x] Algorithm optimization: implementing multithreading (parallel processing of each image on a separate CPU core)
- [x] Implementing a role-based system for working with the application (Administrator/Employee) + registration/authorization
- [x] Implementing exception handling mechanisms (`try/catch`) to protect against failures when working with files and the database
- [x] Adding full English language support (internationalization)
- [ ] Fixing the display of the sorting icon (ORDER BY) in DataGrid headers on small screen sizes
- [ ] Adding automatic calibration of grade thresholds (**First grade** / **Second grade**) based on the reference batch instead of manual entry
- [ ] Adding the ability to save calibration parameters (Sensitivity and Grades) in the report to ensure result traceability
- [ ] Adding a built-in report printing function directly from the application
- [ ] Adding a function to export reports to **PDF** format
- [ ] Implementing hardware acceleration (**GPU**) for image processing (CUDA / OpenCL)
- [ ] Porting the application to other OSes (**Linux**, **macOS**)

The full list of planned features and known issues is available in the [Issues][issues-url] section.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- ! --- LICENSE --->
<!-- ! ----------- --->

## 📄 License <span id="license"></span>

Copyright © 2021–2026 [aLeeTheY](https://github.com/aLeeTheY)

The project is distributed under the [**PolyForm Noncommercial License 1.0.0**](https://polyformproject.org/licenses/noncommercial/1.0.0) and is open for personal use, learning, research, and non-commercial purposes. Commercial use, resale, or use in commercial products is prohibited.

See the [`LICENSE`][license-url] file for detailed information.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- ! --- CONTACT --->
<!-- ! ----------- --->

## 🤝 Contact <span id="contact"></span>

<!-- [![GitHub][GitHub-logo]](https://github.com/aLeeTheY)
[![Telegram][Telegram-logo]](https://t.me/aLeeTheY)
[![Gmail][Gmail-logo]](mailto:aleethey@gmail.com) -->

GitHub: [aLeeTheY](https://github.com/aLeeTheY)
<br/>
Telegram: [@aLeeTheY](https://t.me/aLeeTheY)
<br/>
Email: [aleethey@gmail.com](mailto:aleethey@gmail.com)

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- ! --- ACKNOWLEDGMENTS --->
<!-- ! ------------------- --->

## 💖 Acknowledgments <span id="acknowledgments"></span>

[aLeeTheY](https://github.com/aLeeTheY) expresses gratitude to the developers and communities of the following projects:

- [C#](https://dotnet.microsoft.com/en-us/languages/csharp)
- [.NET Framework](https://learn.microsoft.com/en-us/dotnet/framework/)
- [WPF](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/)
- [Entity Framework](https://learn.microsoft.com/en-us/ef/)
- [SQLite](https://www.sqlite.org/)
- [NLog](https://nlog-project.org/)
- [NuGet](https://www.nuget.org/)
- [NSIS](https://nsis.sourceforge.io/)
- [Visual Studio](https://visualstudio.microsoft.com/)
- [Visual Studio Code](https://code.visualstudio.com/)
- [SQLite DB Browser](https://sqlitebrowser.org/)
- [Figma](https://www.figma.com/)
- [Git](https://git-scm.com/)
- [GitHub](https://github.com/)
- [Shields.io](https://shields.io/) (for the beautiful badges)
- [Custom Icon Badges](https://github.com/DenverCoder1/custom-icon-badges)
- [FFmpeg](https://ffmpeg.org/)
- [ezgif.com](https://ezgif.com/)
- [PolyForm Project](https://polyformproject.org/)

Without these tools, the development of this project would have been **impossible**.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- ! --- MARKDOWN LINKS --->
<!-- ! ------------------ --->

<!-- * shields --->

[contributors-shield]: https://img.shields.io/github/contributors/aLeeTheY/BallScanner-CSharp.svg?style=for-the-badge
[contributors-url]: https://github.com/aLeeTheY/BallScanner-CSharp/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/aLeeTheY/BallScanner-CSharp.svg?style=for-the-badge
[forks-url]: https://github.com/aLeeTheY/BallScanner-CSharp/network/members
[stars-shield]: https://img.shields.io/github/stars/aLeeTheY/BallScanner-CSharp.svg?style=for-the-badge
[stars-url]: https://github.com/aLeeTheY/BallScanner-CSharp/stargazers
[issues-shield]: https://img.shields.io/github/issues/aLeeTheY/BallScanner-CSharp.svg?style=for-the-badge
[issues-url]: https://github.com/aLeeTheY/BallScanner-CSharp/issues
[last-commit-shield]: https://img.shields.io/github/last-commit/aLeeTheY/BallScanner-CSharp?style=for-the-badge
[repo-size-shield]: https://img.shields.io/github/repo-size/aLeeTheY/BallScanner-CSharp?style=for-the-badge
[license-shield]: https://img.shields.io/badge/License-PolyForm_Noncommercial-critical?style=for-the-badge
[license-url]: https://github.com/aLeeTheY/BallScanner-CSharp/blob/master/LICENSE

<!-- * technologies --->

[C#-logo]: https://img.shields.io/badge/C%23-512bd4?style=for-the-badge&logo=c-sharp&logoColor=white
[C#-url]: https://learn.microsoft.com/en-us/dotnet/csharp/
[WPF-logo]: https://img.shields.io/badge/Windows%20Presentation%20Foundation-1f1f1f?style=for-the-badge&logoColor=white
[WPF-url]: https://learn.microsoft.com/en-us/dotnet/desktop/wpf/
[DNF48-logo]: https://img.shields.io/badge/.NET_Framework_4.8-512bd4?style=for-the-badge&logo=.net&logoColor=white
[DNF48-url]: https://learn.microsoft.com/en-us/dotnet/framework/
[EntityFramework-logo]: https://img.shields.io/badge/Entity_Framework-512bd4?style=for-the-badge&logo=.net&logoColor=white
[EntityFramework-url]: https://learn.microsoft.com/en-us/ef/
[NLog-logo]: https://custom-icon-badges.demolab.com/badge/NLog-fff?logo=nlog&style=for-the-badge&logoColor=black
[NLog-url]: https://nlog-project.org/documentation/
[SQLite-logo]: https://img.shields.io/badge/sqlite-07405e?style=for-the-badge&logo=sqlite&logoColor=white
[SQLite-url]: https://www.sqlite.org/docs.html
[NSIS-logo]: https://custom-icon-badges.demolab.com/badge/NSIS-064b4e?logo=nsis&style=for-the-badge&logoColor=white
[NSIS-url]: https://nsis.sourceforge.io/
[XAML-logo]: https://custom-icon-badges.demolab.com/badge/XAML-05aceb?logo=xaml&style=for-the-badge&logoColor=white
[XAML-url]: https://learn.microsoft.com/en-us/dotnet/desktop/wpf/xaml/

<!-- * architecture & patterns --->

[MVVM-logo]: https://custom-icon-badges.demolab.com/badge/MVVM-1f1f1f?&style=for-the-badge&logoColor=white
[MVVM-url]: https://learn.microsoft.com/en-us/dotnet/architecture/maui/mvvm
[MVVMCommandPattern-logo]: https://custom-icon-badges.demolab.com/badge/MVVM%20Command%20Pattern-1f1f1f?&style=for-the-badge&logoColor=white
[MVVMCommandPattern-url]: https://learn.microsoft.com/en-us/archive/msdn-magazine/2013/may/mvvm-commands-relaycommands-and-eventtocommand

<!-- * linters & code format --->

[EditorConfig-logo]: https://custom-icon-badges.demolab.com/badge/EditorConfig-010101?logo=editorconfig&style=for-the-badge
[EditorConfig-url]: https://editorconfig.org/

<!-- * ide & workspace --->

[VSCode-logo]: https://custom-icon-badges.demolab.com/badge/Visual%20Studio%20Code-0d1014?logo=vscode&logoColor=white&style=for-the-badge
[VSCode-url]: https://code.visualstudio.com/
[Figma-logo]: https://img.shields.io/badge/figma-%23F24E1E.svg?style=for-the-badge&logo=figma&logoColor=white
[Figma-url]: https://www.figma.com/
[VisualStudio-logo]: https://custom-icon-badges.demolab.com/badge/Visual%20Studio-212121?logo=visual-studio-2026&style=for-the-badge&logoColor=white
[VisualStudio-url]: https://visualstudio.microsoft.com/
[SQLiteDBBrowser-logo]: https://custom-icon-badges.demolab.com/badge/SQLite%20DB%20Browser-f5f5f5?logo=sqlite-db-browser&style=for-the-badge&logoColor=black
[SQLiteDBBrowser-url]: https://sqlitebrowser.org/

<!-- * version control --->

[Git-logo]: https://img.shields.io/badge/git-F05033?style=for-the-badge&logo=git&logoColor=white
[Git-url]: https://git-scm.com/
[GitHub-logo]: https://img.shields.io/badge/github-121011?style=for-the-badge&logo=github&logoColor=white
[GitHub-url]: https://github.com/

<!-- * quick start tools --->

[VisualStudioInstaller-url]: https://visualstudio.microsoft.com/downloads/
[NuGetCLI]: https://dist.nuget.org/win-x86-commandline/latest/nuget.exe
[NSISInstaller]: https://nsis.sourceforge.io/Download

<!-- * contacts --->

<!-- [GitHub-logo]: https://img.shields.io/badge/github-%23121011.svg?style=for-the-badge&logo=github&logoColor=white
[Telegram-logo]: https://img.shields.io/badge/Telegram-2CA5E0?style=for-the-badge&logo=telegram&logoColor=white
[Gmail-logo]: https://img.shields.io/badge/Gmail-D14836?style=for-the-badge&logo=gmail&logoColor=white -->

<!-- ! --- MARKDOWN ASSETS --->
<!-- ! ------------------- --->

<!-- empty -->
