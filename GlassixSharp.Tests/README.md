# GlassixSharp Tests

This project contains integration tests for the GlassixSharp client library. The tests make real API calls to the Glassix API, so you'll need to provide valid credentials.

## Setup

Create a .runsettings file in the root of the project with the following content:


<?xml version="1.0" encoding="utf-8"?>
<!--Configure Visual Studio to use this file: Test > Configure Run Settings > Select Solution Wide runsettings File-->
<RunSettings>
  <RunConfiguration>
    <EnvironmentVariables>
      <WORKSPACE_NAME></WORKSPACE_NAME>
      <USER_NAME></USER_NAME>
      <API_KEY></API_KEY>
      <API_SECRET></API_SECRET>
    </EnvironmentVariables>
  </RunConfiguration>
</RunSettings>