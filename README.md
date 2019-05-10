# Epinova.NetsPaymentGateway
Epinova's take on a NETS payment gateway API

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Epinova.NetsPaymentGateway&metric=alert_status)](https://sonarcloud.io/dashboard?id=Epinova.NetsPaymentGateway)
[![Build status](https://ci.appveyor.com/api/projects/status/yosbiosrtgf2y317/branch/master?svg=true)](https://ci.appveyor.com/project/Epinova_AppVeyor_Team/epinova-netspaymentgateway/branch/master)
![Tests](https://img.shields.io/appveyor/tests/Epinova_AppVeyor_Team/epinova-netspaymentgateway.svg)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Usage
### Add registry to Structuremap

```
    container.Configure(
        x =>
        {
            x.Scan(y =>
            {
                y.TheCallingAssembly();
                y.WithDefaultConventions();
            });

            x.AddRegistry<Epinova.NetsPaymentGateway.PaymentGatewayRegistry>();
        });
```

### Inject contract and use service

Epinova.NetsPaymentGateway.IPaymentGatewayService describes the service. 