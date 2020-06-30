![Utils Lite](Logo/logo-title.png)

Small tools (object pool, retry logic, automatic mapping etc.) for .NET Standard.

[![Build Status](https://travis-ci.org/balazs-kis/utils-lite.svg?branch=master)](https://travis-ci.org/balazs-kis/utils-lite)
[![Coverage Status](https://coveralls.io/repos/github/balazs-kis/utils-lite/badge.svg?branch=master)](https://coveralls.io/github/balazs-kis/utils-lite?branch=master)
[![Nuget](https://img.shields.io/nuget/v/RedisLite)](https://www.nuget.org/packages/RedisLite)
[![License: MIT](https://img.shields.io/badge/license-MIT-blueviolet)](https://opensource.org/licenses/MIT)
[![pull requests: welcome](https://img.shields.io/badge/pull%20requests-welcome-brightgreen)](https://github.com/balazs-kis/utils-lite/fork)

## Examples

### Date class
The Date class is a simle wrapper around the built-in DateTime struct, which indicates your intent to only use the date component:
```csharp
DateTime myDateTime = new DateTime(2016, 01, 28, 18, 20, 00);
Date justDate = Date.FromDateTime(myDateTime);
Console.WriteLine(justDate.ToString("yyyy-MM-dd"));
```
