This directory is responsible for providing all of
the necessary files needed to add localizations strings
that can/have been translated to other locales.

While the files ending in `.en.resx` is not needed
in general, providing these files can make it easier
for translators that wish to translate to other languages.
Depending on the platform used for localization these may
also be necessary to be available to upload the necessary
strings to that platform.

This directory is not used by default, but have been configured
in method 'ConfigureServices' located in the Startup.cs class file.
