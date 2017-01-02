# ContentIdentifier (cs-cid)

[![Travis CI](https://img.shields.io/travis/multiformats/cs-cid.svg?style=flat-square&branch=master)](https://travis-ci.org/multiformats/cs-cid)
[![AppVeyor](https://img.shields.io/appveyor/ci/tabrath/cs-cid/master.svg?style=flat-square)](https://ci.appveyor.com/project/tabrath/cs-cid)
[![NuGet](https://buildstats.info/nuget/ContentIdentifier)](https://www.nuget.org/packages/ContentIdentifier/)
[![Codecov](https://img.shields.io/codecov/c/github/multiformats/cs-cid/master.svg?style=flat-square)](https://codecov.io/gh/multiformats/cs-cid)
[![Libraries.io](https://img.shields.io/librariesio/github/multiformats/cs-cid.svg?style=flat-square)](https://libraries.io/github/multiformats/cs-cid)

> [CID](https://github.com/ipld/cid) implementation in C#

Supports v0 and v1 of CIDs.

## Table of Contents

- [Install](#install)
- [Usage](#usage)
- [Maintainers](#maintainers)
- [Contribute](#contribute)
- [License](#license)

## Install

    PM> Install-Package ContentIdentifier

## Usage
``` cs
// create v0 cid
var cid = new Cid(MultihashValue);
// create v1 cid
var cid = new Cid(CidCodec.DagCBOR, MultihashValue);
// parse
var cid = Cid.Parse(string/byte[]/Multihash/Cid);
```

## License

[MIT](LICENSE) © 2016 Trond Bråthen
