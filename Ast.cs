enum NodeType
{
    FnDecl,
    FnCall,
}

interface AstNode 
{
    public NodeType Type { get; }
    public List<AstNode> Children { get; set; }
}

class FnDeclNode : AstNode
{
    public NodeType Type => NodeType.FnDecl;
    public List<AstNode> Children { get; set; } = new();
    public string Name { get; set; } = "";
}

class Ast
{
    public void Build(List<Token> tokens)
    {
        foreach (var token in tokens)
        {
            
        }
    }
}