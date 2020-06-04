using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Team
{
    public int id;
    public string name;
    public Dictionary<int, Player> Players = new Dictionary<int, Player>();


    public Team(int id, string name)
    {
        this.id = id;
        this.name = name;
    }
}
