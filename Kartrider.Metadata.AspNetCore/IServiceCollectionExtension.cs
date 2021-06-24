using Microsoft.Extensions.DependencyInjection;

using System;

namespace Kartrider.Metadata.AspNetCore
{
    public static class IServiceCollectionExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public static IServiceCollection AddKartriderMetadata(this IServiceCollection serviceCollection, Action<KartriderMetadataOptions> options)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }
            var kartriderMetadataOptions = new KartriderMetadataOptions();
            options(kartriderMetadataOptions);
            serviceCollection.AddSingleton<IKartriderMetadata>(new KartriderMetadata(kartriderMetadataOptions));
            return serviceCollection;
        }
    }
}
