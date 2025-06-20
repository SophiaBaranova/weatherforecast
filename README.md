<!-- README template from https://github.com/othneildrew/Best-README-Template -->

<!-- Improved compatibility of back to top link -->
<a id="readme-top"></a>

<!-- PROJECT LOGO -->
<div align="center">
  <a href="https://github.com/SophiaBaranova/weatherforecast.git">
    <img src="https://github.com/SophiaBaranova/weatherforecast/blob/development/demoScreenshots/icon.JPG" alt="Logo" width="120" height="120">
  </a>
</div>
<h1 align="center">Weather Forecast</h1>



<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#contact">Contact</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project


This application allows the client user to view the weather forecast for a selected region and time period, filter data, calculate the average value of weather conditions and save the report in .docx format.
Text content is accompanied by pictures to ease the perception of information.

The administrator user can manage the database content: add, edit and delete entries using a convenient GUI, which also helps prevent providing any invalid data.

As a storage it uses a database on a local server, but you can change this according to your needs, simply replacing the `connectionString` in the [App.config][appconfig-url].

<p align="right">(<a href="#readme-top">back to top</a>)</p>



### Built With


* [.NET][.NET-url]
* [C#][Csharp-url]
* [WPF][WPF-url]
* [MySQL][MySQL-url]

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- GETTING STARTED -->
## Getting Started


### Prerequisites

1. Install MySQL Server (version 8.0+)
   
   [Download][MySQLServer-download]
   
2. Change the `root` password to `SnY1B4Gho`
   ```sh
   mysql> ALTER USER 'root'@'localhost' IDENTIFIED BY SnY1B4Gho;
   ```
4. Launch MySQL client
   * Use MySQL Command-Line Client (bundled with MySQL Server)
   * Use Graphical Tool (e.g. MySQL Workbench, DBeaver etc.)
5. Run the script [weatherdb.sql][script-url]
   * In CLI:
       ```sh
       mysql -u root -p < weatherdb.sql
       ```
   * In GUI:

     Open the script file and execute


### Installation

1. Download files from [Setup WeatherForecast/Debug][setup-url]
2. Run the launcher `setup`

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- USAGE EXAMPLES -->
## Usage


To start working with Weather Forecast, you need to log in. All users are registered as clients by default, so if you want to have admin privilleges, use one of the admin accounts we've already created in the database or create your own directly there.

<div align="center">
    <img src="https://github.com/SophiaBaranova/weatherforecast/blob/development/demoScreenshots/authorization.jpg" height="400">
</div>

Then you will be asked to choose the region - only from options available in the database.

<div align="center">
    <img src="https://github.com/SophiaBaranova/weatherforecast/blob/development/demoScreenshots/chooseRegion.jpg" height="400">
</div>

The later workflow depends on the type of your account.


### Client

Choose date period.

<div align="center">
    <img src="https://github.com/SophiaBaranova/weatherforecast/blob/development/demoScreenshots/searchParam.jpg" height="400">
</div>

If the period is valid and data for it exists, you will see a calendar with a short description of weather conditions. Press on a particular day to see more details.

<div align="center">
    <img src="https://github.com/SophiaBaranova/weatherforecast/blob/development/demoScreenshots/calendar.jpg" height="400">
    <img src="https://github.com/SophiaBaranova/weatherforecast/blob/development/demoScreenshots/day.jpg" height="400">
</div>

You can filter the information and calculate the average value of weather conditions by pressing corresponding buttons in the calendar window.

<div align="center">
    <img src="https://github.com/SophiaBaranova/weatherforecast/blob/development/demoScreenshots/filter.jpg" height="400">
    <img src="https://github.com/SophiaBaranova/weatherforecast/blob/development/demoScreenshots/calculation.jpg" height="400">
</div>

To save the report, press the download button and choose the location.


### Administrator

You will see the table with all available data for chosen region. Use the dropdown to select a particular month.

<div align="center">
    <img src="https://github.com/SophiaBaranova/weatherforecast/blob/development/demoScreenshots/table.jpg" height="400">
</div>

You can add or edit entries by pressing corresponding buttons.

<div align="center">
    <img src="https://github.com/SophiaBaranova/weatherforecast/blob/development/demoScreenshots/add.jpg" height="400">
    <img src="https://github.com/SophiaBaranova/weatherforecast/blob/development/demoScreenshots/edit.jpg" height="400">
</div>

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- CONTACT -->
## Contact

Sophia Baranova

[@SophiaBaranova](https://github.com/SophiaBaranova) - sonya.brnv@gmail.com

Anna Chernyavska

[@atomicher](https://github.com/atomicher) - annchernyavskaya55@gmail.com

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- MARKDOWN LINKS -->
[db-url]: https://github.com/SophiaBaranova/weatherforecast/blob/master/weatherdb.sql
[appconfig-url]: https://github.com/SophiaBaranova/weatherforecast/blob/master/weatherforecast/App.config
[.NET-url]: https://learn.microsoft.com/uk-ua/dotnet/
[Csharp-url]: https://learn.microsoft.com/uk-ua/dotnet/csharp/
[WPF-url]: https://learn.microsoft.com/uk-ua/dotnet/desktop/wpf/
[MySQL-url]: https://www.mysql.com/
[MySQLServer-download]: https://dev.mysql.com/downloads/mysql/
[script-url]: https://github.com/SophiaBaranova/weatherforecast/blob/master/weatherdb.sql
[setup-url]: https://github.com/SophiaBaranova/weatherforecast/tree/master/Setup%20WeatherForecast/Debug
