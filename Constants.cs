namespace CS341GroupProject;
/// <summary>
/// Strings and warnings that come up repeatedly.
/// </summary>
public static class Constants
{
    public const double MAP_STARTING_LATITUDE = 44.0263;
    public const double MAP_STARTING_LONGITUDE = -88.5507;

    //***Database connection information***
    public const string DB_HOST = "cs341-labserver-13086.5xj.cockroachlabs.cloud";
    public const int DB_PORT = 26257;
    public const string DB_NAME = "project_group_3_db";
    public const string DB_APP = "group_project"; //optional
    public const string DB_USER = "hill_joshuataylor_gm";
    public const string DB_PASS = "iZyAvF0iGFv553Ahf-yvGg";

    //***SQL Queries and Commands***
    //MapPage
    public const string SQL_INSERT_MAP_TABLE_STRING = "INSERT INTO map (latitude, longitude, plant_genus, plant_specific_epithet) VALUES (@latitude, @longitude, @genus, @epithet);";
    public const string SQL_GET_MAP_PIN_DATA_STRING = "SELECT id, latitude, longitude, plant_genus, plant_specific_epithet FROM map;";

    //
}