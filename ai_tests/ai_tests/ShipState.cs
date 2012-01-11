using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ai_tests
{
    class ShipState
    {
        public double xpos;
        public double ypos;
        public double angle;
        public double xspeed;
        public double yspeed;
        static Stack<ShipState> pool = new Stack<ShipState>();
        
        public static ShipState getFromPool()
        {
            if (pool.Count > 0)
            {
                ShipState ss = pool.Pop();                
                ss.clear();                
                return ss;
            }
            else
            {
                ShipState ss = new ShipState();
                ss.clear();
                return ss;
            }
        }
        public void putToPool()
        {
            pool.Push(this);
        }
        public void clear()
        {   
            xpos = 0;
            ypos = 0;
            angle = 0;
            xspeed = 0;
            yspeed = 0;
        }
        public void set(ShipState other)
        {            
            xpos = other.xpos;
            ypos = other.ypos;
            angle = other.angle;
            xspeed = other.xspeed;
            yspeed = other.yspeed;
        }
        public void move(double thrust, double turn)
        {
            angle += turn;
            double xthrust = thrust * Math.Cos(angle);
            double ythrust = thrust * Math.Sin(angle);
            xspeed += xthrust;
            yspeed += ythrust;            
            xpos += xspeed;
            ypos += yspeed;
        }
    }
}
