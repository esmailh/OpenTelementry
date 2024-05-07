
# OpenTelementry
exec this command and build docker image:


```bash
  docker build -t sampleopen -f Dockerfile .
```

exec this command and run docker compose

```bash
  docker compose up
```

## API Reference

#### Run app

```http
  http://localhost:5028/api/SampleOpenTelementry/weatherforecast/weather
```

#### JaegerUi

```http
 http://localhost:16686/
```

