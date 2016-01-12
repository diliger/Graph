using Starcounter;
using Simplified.Ring6;

namespace Graph {
    partial class GraphPage : Page, IBound<Simplified.Ring6.Graph> {
        [GraphPage_json.Values]
        public partial class GraphPageValues : Json, IBound<GraphValue> {
        }
    }
}
