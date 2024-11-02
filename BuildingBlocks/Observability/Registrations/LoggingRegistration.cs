using Elastic.Channels;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Observability.Options;
using Serilog;
using Serilog.Formatting.Compact;

namespace Observability.Registrations;

public static class LoggingRegistration
{
    public static IHostBuilder UseLogging(this IHostBuilder hostBuilder, IConfiguration configuration)
    {
        {
            // var seqOptions = configuration.GetSection("Seq").Get<SeqOptions>();
            var elkOptions = configuration.GetSection(ElkOptions.SectionName).Get<ElkOptions>();

            // if (seqOptions.Enabled)
            // {
            //     services.AddSeqLogger(seqOptions);
            // }

            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console(new CompactJsonFormatter());
            
            if (elkOptions is { Enabled: true })
            {
                logger.WriteTo.Elasticsearch([new Uri(elkOptions.ElasticSearchUrl)], opts =>
                {
                    opts.DataStream = new DataStreamName("logs", "microservice", "localhost");
                    opts.BootstrapMethod = BootstrapMethod.Failure;
                    opts.ConfigureChannel = channelOpts =>
                    {
                        channelOpts.BufferOptions = new BufferOptions
                        {
                        };
                    };
                }, transport =>
                {
                    // transport.Authentication(new BasicAuthentication(username, password)); // Basic Auth
                    // transport.Authentication(new ApiKey(base64EncodedApiKey)); // ApiKey
                });
            }
            
            hostBuilder.UseSerilog(logger.CreateLogger());

            return hostBuilder;
        }
    }
}