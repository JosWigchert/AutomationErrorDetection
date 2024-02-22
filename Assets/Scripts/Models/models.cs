using System;
using Newtonsoft.Json;
using System.Collections.Generic;

public class Mask
{
    public int width { get; set; }
    public int height { get; set; }
}

public class Class
{
    public int id { get; set; }
    public string name { get; set; }
}

public class Location
{
    public int x { get; set; }
    public int y { get; set; }
    public bool isEmpty { get; set; }
}

public class Size
{
    public int width { get; set; }
    public int height { get; set; }
    public bool isEmpty { get; set; }
}

public class Bounds
{
    public int x { get; set; }
    public int y { get; set; }
    public int width { get; set; }
    public int height { get; set; }
    public Location location { get; set; }
    public Size size { get; set; }
    public bool isEmpty { get; set; }
    public int top { get; set; }
    public int right { get; set; }
    public int bottom { get; set; }
    public int left { get; set; }
}

public class Box
{
    public Mask mask { get; set; }
    public Class @class { get; set; }
    public Bounds bounds { get; set; }
    public double confidence { get; set; }
}

public class Image
{
    public int width { get; set; }
    public int height { get; set; }
    public bool isEmpty { get; set; }
}

public class Speed
{
    public string preprocess { get; set; }
    public string inference { get; set; }
    public string postprocess { get; set; }
}

public class Root
{
    public List<Box> boxes { get; set; }
    public Image image { get; set; }
    public Speed speed { get; set; }
}