# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Initial development

- `Remote` class for interacting with the Voicemeeter Remote API. See [README](README.md) for usage details.
  - `Login()` and `Logout()` methods to manage API sessions.
  - `LoginStatus` property to check the current login state.
  - `LoggedIn` bool property to simplify login status checks.
  - `Run(app)` method to launch VB-Audio applications.

  - `GetKind()` method to get the running Voicemeeter kind.
  - `RunningKind` property to safely check the currently running Voicemeeter kind.
  - `GetVersion()` method to get the running Voicemeeter version.
  - `RunningVersion` property to safely check the currently running Voicemeeter version.

  - `ParamsDirty()` method to check if parameters have changed.
