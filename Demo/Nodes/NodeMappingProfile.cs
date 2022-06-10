using DotNode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Nodes;
internal class NodeMappingProfile
{
    public NodeMappingProfile()
    {
        //var catalogNode = new CatalogNode();
        //var policyNode = new PolicyNode();

        //var asd = new PropertyConfiguration<Product, PolicyId>();
        //asd.Create(x => x.Price, x => x.Id);

        //var connection = new CatalogPolicyConnection();
        //connection.Connect(x => x.Price, x => x.Id, x => (int)x);
        //connection.Connect(x => x, x => x, x => new PolicyId((int)x.Price));

        //connection.Connect1((product, policyId) => policyId = policyId with { Id = (int)product.Price });

        //connection.Connect((product, policyId) => policyId with { Id = (int)product.Price });
    }
}


//public class CatalogPolicyConnection : Connection<CatalogNode, Product, PolicyNode, PolicyId>
//{
//    //public Func<Product, PolicyId, PolicyId>? ConnectFunction { get; private set; }

//    //public override void Connect<T1, T2>(Func<Product, T1> prop1, Func<PolicyId, T2> prop2, Func<T1, T2> connectFunction)
//    //{

//    //}

//    public void Connect1(Action<Product, PolicyId> connectFunction)
//    {

//    }

//    //public void Connect2(Func<Product, PolicyId, PolicyId> connectFunction)
//    //{
//    //    ConnectFunction = connectFunction;
//    //}
//}



//public class CatalogPolicyConnection : IConnection<CatalogNode, Product, PolicyNode, PolicyId>
//{
//    public void Connect<T1, T2>(Func<Product, T1> prop1, Func<PolicyId, T2> prop2, Func<T1, T2> connectFunction)
//    {
//        throw new NotImplementedException();
//    }
//}