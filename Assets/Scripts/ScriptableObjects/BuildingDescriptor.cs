using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CityBuilder;

namespace CityBuilder
{
    [CreateAssetMenu(fileName = "BuildingInfo", menuName = "CityBuilder/New BuildingInfo")]
    public class BuildingDescriptor : ScriptableObject
    {
        public enum Category
        {
            None,
            Building,
            Roadway,
            PowerLine,
            Aqueduct
        }

        [ShowOnly] public int uuid = 0;
        public Category category = Category.None;

        [MenuItem("CityBuilderTools/Generate Building UUIDs")]
        public static void GenerateBuildingUUIDs()
        {
            const int maxAttemps = 100;
            int newID = 0;
            int count = 0;

            string[] guids = AssetDatabase.FindAssets("t:" + typeof(BuildingDescriptor));
            BuildingDescriptor[] descriptorsArray = new BuildingDescriptor[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                descriptorsArray[i] = AssetDatabase.LoadAssetAtPath<BuildingDescriptor>(path);
            }

            Dictionary<int, BuildingDescriptor> descriptorsHashMap = new Dictionary<int, BuildingDescriptor>();
            foreach (var element in descriptorsArray)
            {
                if (element.uuid != 0 && !descriptorsHashMap.ContainsKey(element.uuid))
                {
                    descriptorsHashMap.Add(element.uuid, element);
                    continue;
                }

                newID = 0;
                count = 0;
                while (newID == 0)
                {
                    if (count > maxAttemps)
                    {
                        Debug.LogError("Error assigning UUID");
                        break;
                    }

                    count++;
                    newID = UnityEngine.Random.Range(0, int.MaxValue);
                    if (descriptorsHashMap.ContainsKey(element.uuid))
                        newID = 0;
                }
                element.uuid = newID;
                descriptorsHashMap.Add(element.uuid, element);
            }
            AssetDatabase.SaveAssets();
        }
    }
}

//private enum Type
//{
//    None = 0,

//    ResidenceSmall = 1,
//    ResidenceMedium = 2,
//    ResidenceLarge = 3,
//    ResidenceVeryLarge = 4,

//    CommerceSmall = 10,
//    CommerceMedium = 11,
//    CommerceLarge = 12,

//    OfficeSmall = 20,
//    OfficeMedium = 21,
//    OfficeLarge = 22,

//    PowerPlantCoal = 30,
//    PowerPlantHydro = 31,
//    PowerPlantSolar = 32,
//    PowerPlantWind = 33,
//    PowerPlantNuclear = 34,

//    TransitStationSubway = 40,
//    TransitStationTrain = 41,

//    TransportAirport = 50,
//    TransportSeaport = 51,

//    ServiceHospital = 60,
//    ServiceFirefighter = 61,
//    ServicePolice = 62,
//    ServicePrison = 63,
//    ServicePostOffice = 64,

//    SchoolElementary = 70,
//    SchoolSecondary = 71,
//    SchoolUniversity = 72,

//    EntertainmentMuseum = 80,
//    EntertainmentMovieTheatre = 81,
//    EntertainmentTheatre = 82,
//    EntertainmentStadium = 83,
//    EntertainmentAmusementPark = 84,

//    ParkSmall = 90,
//    ParkMedium = 91,
//    ParkLarge = 92,

//    TourismSkiResort = 100,
//    TourismBeachResort = 101,

//    BasicRoad = 200,
//    Highway = 201,
//    Railway = 202,
//    Subway = 203,
//}
