# Epinova.NetsPaymentGateway
Epinova's take on a NETS payment gateway API

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