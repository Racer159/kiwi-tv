using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Kiwi_TV.Helpers
{
    /// <summary>
    /// A helper to deal with the custom category list file
    /// </summary>
    class CategoryHelper
    {
        /* Helper to load a list of categories from the categories file */
        public async static Task<List<string>> LoadCategories()
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            StorageFolder currentFolder;

            // Determines the correct folder based on a user's sync settings
            if (localSettings.Values["syncData"] is bool && !(bool)localSettings.Values["syncData"])
            {
                currentFolder = ApplicationData.Current.LocalFolder;
            }
            else
            {
                currentFolder = ApplicationData.Current.RoamingFolder;
            }


            IStorageItem categoriesItem = await currentFolder.TryGetItemAsync("categories.txt");
            StorageFile categoriesFile;

            // Determines if a categories file exists, and if not copies in the defaults
            if (categoriesItem == null || !(categoriesItem is StorageFile))
            {
                categoriesFile = await currentFolder.CreateFileAsync("categories.txt", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteLinesAsync(categoriesFile, System.IO.File.ReadAllLines("Data/categories.txt"));
            }
            else
            {
                categoriesFile = (StorageFile)categoriesItem;
            }

            List<string> allCategories = (await FileIO.ReadLinesAsync(categoriesFile)).ToList();

            return allCategories;
        }

        /* Helper to save a list of categories to the categories file */
        public async static Task SaveCategories(List<string> categories)
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            StorageFolder currentFolder;

            // Determines the correct folder based on a user's sync settings
            if (localSettings.Values["syncData"] is bool && !(bool)localSettings.Values["syncData"])
            {
                currentFolder = ApplicationData.Current.LocalFolder;
            }
            else
            {
                currentFolder = ApplicationData.Current.RoamingFolder;
            }

            StorageFile categoriesFile = await currentFolder.CreateFileAsync("categories.txt", CreationCollisionOption.ReplaceExisting);

            string file = "";
            foreach (string category in categories)
            {
                file += (category + "\n");
            }

            await FileIO.WriteTextAsync(categoriesFile, file);
        }
    }
}
