   # Firebase Sample
   
   This sample was created for sharing the code used in my blogging example on [my consulting website](https://JeremySnyder.Consulting)
   * [Security Basics, Part 1: Quick And Dirty Security With Firebase](https://jeremysnyder.consulting/technology/quick-and-dirty-security-with-firebase)
   * [Security Basics, Part 2: Simple Database Structure for Security](https://jeremysnyder.consulting/technology/simple-database-structure-for-security)
   * [Idempotent Database Scripting](https://jeremysnyder.consulting/technology/master-sql-scripting-structure)
   * [Security Basics, Part 3: .NET Project Structure](https://jeremysnyder.consulting/technology/security-basics-part-3)
   
   Usage of these files is freely available as long as you mark a reference to the original site you found this within your code sample, either by the GitHub account or the referencing blog posting with the full URLs.
   
   ## JeremySnyder.Security
   This module loads a configuration segment and connects to the configured firebase application.

   Packages used
   * FirebaseAdmin / 2.0.0
   * Newtonsoft.Json / 12.0.3

   
   ## JeremySnyder.Common
   This module contains commonly used calls that may be reused within a variety of contexts
   
   Packages used
   * Microsoft.Extensions.Configuration / 5.0.0
   * Microsoft.Extensions.Configuration.FileExtensions / 5.0.0
   * Microsoft.Extensions.Configuration.Json / 5.0.0
   * Newtonsoft.Json / 12.0.3
   
