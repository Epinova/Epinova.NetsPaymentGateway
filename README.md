# Epinova.NetsPaymentGateway
Epinova's take on a NETS payment gateway API.

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Epinova.NetsPaymentGateway&metric=alert_status)](https://sonarcloud.io/dashboard?id=Epinova.NetsPaymentGateway)
[![Build status](https://ci.appveyor.com/api/projects/status/yosbiosrtgf2y317/branch/master?svg=true)](https://ci.appveyor.com/project/Epinova_AppVeyor_Team/epinova-netspaymentgateway/branch/master)
![Tests](https://img.shields.io/appveyor/tests/Epinova_AppVeyor_Team/epinova-netspaymentgateway.svg)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Getting Started

### Configuration

No configuration via config files are needed. The [MerchantInfo](stc/MerchantInfo.cs) passed in to each method described in
[IPaymentGatewayService](src/IPaymentGatewayService.cs) dictates merchant info and environment to be used (test or production).

### Set up payment transact example:

```csharp
public class NetsProcessor : IPaymentGatewayProcessor
{
    private readonly IPaymentGatewayService _paymentGateway;

    public NetsProcessor(IPaymentGatewayService paymentGateway)
    {
        _paymentGateway = paymentGateway;
    }

    public async Task<PaymentSetupModel> InitializePaymentAsync(string orderNumber, decimal totalAmount)
    {
	    var request = new RegisterRequest
        {
            Amount = totalAmount,
            CurrencyCode = "NOK",
            OrderDescription = "Order description. Line by line.",
            OrderNumber = orderNumber,
            RedirectUrl = new Uri($"https://absolute.url/to/your/site/NetsCallBack?orderNumber={orderNumber}")
        };
		//Send in specific payment method, or leave blank to use default payment methods defined in MerchantInfo model.
        //request.PaymentMethods.Add("Vipps");

        RegisterResponse response = await _paymentGateway.RegisterAsync(GetMerchantInfo(), request);

        return new PaymentSetupModel(registerResponse.TransactionId, registerResponse.TerminalUrl);
    }
 
    private static MerchantInfo GetMerchantInfo()
    {
        return new MerchantInfo
        {
            IsTestEnvironment = Boolean.Parse(ConfigurationManager.AppSettings["Nets.IsTestMode"] ?? "false"),
            MerchantId = ConfigurationManager.AppSettings["Nets.MerchantId"],
            Token = ConfigurationManager.AppSettings["Nets.Token"],
            DefaultPaymentMethods = (ConfigurationManager.AppSettings[$"Nets.DefaultPaymentMethods"] ?? String.Empty)
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToArray(),
            ValidCallbackIPs = (ConfigurationManager.AppSettings["Nets.ValidCallbackIPs"]?.Split(',') ?? Enumerable.Empty<string>())
                .Select(ip => IPAddress.TryParse(ip.Trim(), out IPAddress parsedIp) ? parsedIp : IPAddress.None)
                .Where(ip => !ip.Equals(IPAddress.None)).ToArray()
            };
        }
}
```

web.config:
```xml
<configuration>
    <appSettings>
        <add key="Nets.IsTestMode" value="true" />
        <add key="Nets.MerchantId" value="1337" />
        <add key="Nets.Token" value="xyzabc" />
        <add key="Nets.DefaultPaymentMethods" value="Visa, MasterCard" />
        <add key="Nets.ValidCallbackIPs" value="127.0.0.1" />
    <appSettings>
</configuration>
```

### Add registry to IoC container

if using Structuremap:
```csharp
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

If you cannot use the [structuremap registry](src/PaymentGatewayRegistry.cs) provided with this module,
you can manually set up [PaymentGatewayService](src/PaymentGatewayService.cs) for [IPaymentGatewayService](src/IPaymentGatewayService.cs)


### Inject contract and use service

[Epinova.NetsPaymentGateway.IPaymentGatewayService](src/IPaymentGatewayService.cs) describes the NETS payment gateway service.

### Prerequisites

* [EPiServer.Framework](http://www.episerver.com/web-content-management) >= v11.1 for logging purposes.
* [Automapper](https://github.com/AutoMapper/AutoMapper) >= v8.0 for mapping models.
* [StructureMap](http://structuremap.github.io/) >= v4.7 for registering service contract.

### Installing

The module is published on nuget.org.

```bat
nuget install Epinova.NetsPaymentGateway
```

## Built With

* .Net Framework 4.6.2

## Authors

* **Tarjei Olsen** - *Initial work* - [apeneve](https://github.com/apeneve)

See also the list of [contributors](https://github.com/Epinova/Epinova.NetsPaymentGateway/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details

## Further reading

[Netaxept Technical documentation](https://shop.nets.eu/web/partners/home)























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