using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using TourPlanner.DAL.Postgres;

[TestFixture]
public class DatabaseConnectionTests
{
    private string _connectionString;

    [SetUp]
    public void Setup()
    {
        _connectionString = "Host=localhost;Port=5432;Database=tourplannerdb;Username=postgres;Password=postgres";
    }

    [Test]
    public void CanConnectToDatabase()
    {
        var connectionString = "Host=localhost;Port=5432;Database=tourplannerdb;Username=postgres;Password=postgres";

        using var connection = new Npgsql.NpgsqlConnection(connectionString);
        connection.Open();

        Assert.That(connection.State, Is.EqualTo(System.Data.ConnectionState.Open));
    }
}
