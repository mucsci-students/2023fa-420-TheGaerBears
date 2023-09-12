﻿using SpellingBee.SetUpSpellingBee.src.main;
using System;
using SQLitePCL;
internal class Program
{
    static void Main(string[] args)
    {
        // Initialize SQLitePCL
        Batteries.Init();

        Console.WriteLine("Start here!");
        CreateDatabase db = new CreateDatabase();
        //db.AddPangramTable("pangrams");
        db.PrintCount("pangrams");
    }
}