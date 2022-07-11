using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OffenseDefense.Client
{
  class Colors
  {
    private Color blue = new Color(0, 0, 255);
    private Color red = new Color(255, 0, 0);
    private Color green = new Color(0, 255, 0);
    private Color orange = new Color(255, 165, 0);
    private Color yellow = new Color(255, 255, 0);
    private Color pink = new Color(255, 192, 203);
    private Color purple = new Color(128, 0, 128);
    private Color white = new Color(255, 255, 255);

    public static Dictionary<string, Color> list = new Dictionary<string, Color>() {
            {"blue", blue},
            {"red", red},
            {"green", green},
            {"orange", orange},
            {"yellow", yellow},
            {"pink", pink},
            {"purple", purple},
            {"white", white}
        };
  }
}
