[![License](https://img.shields.io/github/license/zetroot/HealtzHealthcheck)](https://github.com/zetroot/HealtzHealthcheck/blob/master/LICENSE)
[![CI status](https://github.com/zetroot/HealtzHealthcheck/actions/workflows/dotnet.yml/badge.svg)](https://github.com/zetroot/HealtzHealthcheck/actions/workflows/dotnet.yml)
[![codecov](https://codecov.io/gh/zetroot/HealtzHealthcheck/branch/master/graph/badge.svg)](https://codecov.io/gh/zetroot/HealtzHealthcheck)
[![Nuget](https://img.shields.io/nuget/v/ThrottledHealthCheck)](https://www.nuget.org/packages/ThrottledHealthCheck/)
[![Nuget](https://img.shields.io/nuget/dt/ThrottledHealthCheck)](https://www.nuget.org/packages/ThrottledHealthCheck/)
# Троттлящие Хелсчеки
Распилил монолит и получил распределенный монолит или даже кучку микросервисов?

Добавил хелсчеки? И даже нацелил их на `/health` друг друга, чтобы работал error propagation?

Избавился от циклических зависимостей и все даже работает?

Но все равно будто бы чего то не хватает, откуда эти 8-10 рпс на самых важных сервисах?

## Мультипликация запросов
Каждый раз когда мы дергаем эндпоинт `/health` у сервиса, он идет опрашивать все имеющиеся у него хелсчеки. А они идут опрашивать свои. А те - свои. В итоге этот снежный ком опросов растет пока на вашем сервере с прикладом не закончатся сокеты или коммутатор не скажет, что все, хватит, 10 Гбит\с хелсчеков достаточно. А ведь хочется не просто сетевую связанность проверять, но и знать о здоровье своих зависимостей.

## Троттлинг опроса
Просто не надо каждый раз когда кто-то спрашивает "как дела?" действительно смотреть на свои "дела" и рассказывать. За 15 секунд врядли что-то поменялось, просто отдайте предыдущий ответ.
# Ну и как же?
А очень просто. Надо заменить дефолтную реализацию `HealthCheckService` (она так и называется `DefaultHealthCheckService` и живёт [тут](https://github.com/dotnet/aspnetcore/blob/06a1f45f79c300c732b0171ce6318a130d05a083/src/HealthChecks/HealthChecks/src/DefaultHealthCheckService.cs)).
В этой репе даже есть готовые методы для вливания в DI!
```csharp
var healthEndpoint = new Uri("/health", UriKind.Relative);
var hcBuilder = builder.Services.AddHealthChecks()
    .AddUrlGroup(new Uri(new Uri("http://ololo:42"), healthEndpoint), name: "ololo")
    .AddUrlGroup(new Uri(new Uri("http://azaza:9000"), healthEndpoint), name: "azaza")
    .AddUrlGroup(new Uri(new Uri("http://ya_voditel_nlo:1234"), healthEndpoint), name: "ufoDriver")
    .ForwardToPrometheus();
 
builder.Services.ThrottleHealthChecks(); // <<== this is the magic, yeah!
```

# Благодарности
Я код скопирнул из [dotnet/aspnetcore](https://github.com/dotnet/aspnetcore). Спасибо им!
![I stole ur code - It's not mine](not_my.png "Not my code")
