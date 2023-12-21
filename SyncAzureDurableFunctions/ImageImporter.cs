using MRI.PandA.Syncs.Data.Configuration;
using MRI.PandA.Syncs.Functions.MixApis.Schema;
using MRI.PandA.Syncs.Functions.MixApis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncAzureDurableFunctions
{
    public class ImageImporter
    {
        public void ImportImages(string propertyId, string clientId, string[] ids, FeedConfig feedConfig)
        {
            mri_spmrm_getrmimages mediaImages = null;
            var length = ids[0].IndexOf(":") == -1 ? ids[0].Length : ids[0].IndexOf(":");

            try
            {
                mediaImages = MixApiClient.CallApi<mri_spmrm_getrmimages>(
                    new RmImagesApi(ids[0].Substring(0, length)),
                    null,
                    feedConfig.WebServiceUrl.TrimEnd('/'),
                    clientId,
                    feedConfig.ClientDatabase,
                    feedConfig.WebServiceUsername,
                    feedConfig.WebServicePassword,
                    feedConfig.MixApiKey);
            }
            catch
            {
                //don't stop processing because they are allowed to not have images
            }

            if (mediaImages == null)
            {
                // log.Debug($"Got NULL images.");
            }
            //else
            //{
            //    // log.Debug($"Got {mediaImages.entry.Length} images.");
            //    var images = BuildUnitMarketRentsList(mediaImages);
            //    SaveImagesToJsonFile(propertyId, images, new BlobContainerClient(feedConfig.AzureStorageConnectionString, feedConfig.AzureStorageContainerName), guid);
            //}
        }

        //private static List<Image> BuildUnitMarketRentsList(mri_spmrm_getrmimages images)
        //{
        //    return images.entry.Select(i => new Image()
        //    {
        //        UnitID = i.UnitID,
        //        BuildingID = i.BuildingID,
        //        PropertyID = i.PropertyID,
        //        FloorplanID = i.FloorplanID,
        //        ExternalLink = i.ExternalLink,
        //        IsFloorPlan = i.IsFloorPlan      
        //    }).ToList();
        //}

        //private async static void SaveImagesToJsonFile(string propertyId, List<Image> images, BlobContainerClient blobContainerClient, Guid guid)
        //{
        //    var json = JsonConvert.SerializeObject(images, new JsonSerializerSettings { Formatting = Newtonsoft.Json.Formatting.Indented, NullValueHandling = NullValueHandling.Ignore });

        //    //using(var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
        //    //{
        //    //    await blobContainerClient.UploadBlobAsync($"{guid}/{propertyId}/images", stream);
        //    //}

        //    var localDataPath = @"C:\MRIPandASyncsTestData\Data\Images";
        //    Directory.CreateDirectory(localDataPath);
        //    var savePath = Path.Combine(localDataPath, propertyId);
        //    if(!string.IsNullOrWhiteSpace(json))
        //    {
        //        await File.WriteAllTextAsync(savePath, json);
        //    }
        //}
    }
}
