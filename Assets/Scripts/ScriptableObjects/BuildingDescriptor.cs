using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuilder
{
    [CreateAssetMenu(fileName = "BuildingInfo", menuName = "CityBuilder/New BuildingInfo")]
    public class BuildingDescriptor : ScriptableObject
    {
        [System.Serializable]
        public enum Category
        {
            None,
            Building,
            Roadway,
            PowerLine,
            Aqueduct
        }

        public int uuid = 0;
        public Category category = Category.None;
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
