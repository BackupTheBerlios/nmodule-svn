options
{
	language="CSharp"; 
	namespace="NModule.Dependency.Parser";
}
  
class DepParser extends Parser;

options {
	k=3;
}

// Rebuild this to generate the dep tree
expr[DepNode root]
	: cexpr[root, true]
	;

// LPAREN! ((NOTO^|AND^|OR^|XOR^|OPT^) (oexpr|cexpr)+) RPAREN!
cexpr[DepNode parent,bool root]
		: notexpr[parent, root]
		| andexpr[parent, root]
		| orexpr[parent, root]
		| xorexpr[parent, root]
		| optexpr[parent, root]
    | oexpr[parent, root]
    ;

notexpr[DepNode parent, bool root]
{ DepNode child = !(root)? parent.CreateNewChild() : parent; child.DepOp = DepOps.Not; }
	: LPAREN! NOTO (oexpr[child, false]|({DepNode nchild = parent.CreateNewChild(); } cexpr[nchild, true]))+ RPAREN!
	;

andexpr[DepNode parent, bool root]
{ DepNode child = !(root)? parent.CreateNewChild() : parent; child.DepOp = DepOps.And; }
	: LPAREN! AND (oexpr[child, false]|({DepNode nchild = parent.CreateNewChild(); } cexpr[nchild, true]))+ RPAREN!
	;

orexpr[DepNode parent, bool root]
{ DepNode child = !(root)? parent.CreateNewChild() : parent; child.DepOp = DepOps.Or; }
	: LPAREN! OR (oexpr[child, false]|({DepNode nchild = parent.CreateNewChild(); } cexpr[nchild, true]))+ RPAREN!
	;

xorexpr[DepNode parent, bool root]
{ DepNode child = !(root)? parent.CreateNewChild() : parent; child.DepOp = DepOps.Xor; }
	: LPAREN! XOR (oexpr[child, false]|({DepNode nchild = parent.CreateNewChild(); } cexpr[nchild, true]))+ RPAREN!
	;


optexpr[DepNode parent, bool root]
{ DepNode child = !(root)? parent.CreateNewChild() : parent; child.DepOp = DepOps.Opt; }
	: LPAREN! OPT (oexpr[child, false]|({DepNode nchild = parent.CreateNewChild(); } cexpr[nchild, true]))+ RPAREN!
	;

// LPAREN! ((EQ^|NEQ^|LTE^|LS^|GTE^|GT^|LD^) iexpr) RPAREN!
oexpr[DepNode parent, bool root]
	: eqexpr[parent, root]
	| neqexpr[parent, root]
	| lteexpr[parent, root]
	| ltexpr[parent, root]
	| gteexpr[parent, root]
	| gtexpr[parent, root]
	| ldexpr[parent, root]
	;
	
eqexpr[DepNode parent, bool root]
{ DepNode child = (!root)? parent.CreateNewChild() : parent; child.DepOp = DepOps.Equal; }
	: LPAREN! EQ iexpr[child] RPAREN!
	;

neqexpr[DepNode parent, bool root]
{ DepNode child = (!root)? parent.CreateNewChild() : parent; child.DepOp = DepOps.NotEqual; }
	: LPAREN! NEQ iexpr[child] RPAREN!
	;

lteexpr[DepNode parent, bool root]
{ DepNode child = (!root)? parent.CreateNewChild() : parent; child.DepOp = DepOps.LessThanEqual; }
	: LPAREN! LTE iexpr[child] RPAREN!
	;

ltexpr[DepNode parent, bool root]
{ DepNode child = (!root)? parent.CreateNewChild() : parent; child.DepOp = DepOps.LessThan; }
	: LPAREN! LS iexpr[child] RPAREN!
	;

gteexpr[DepNode parent, bool root]
{ DepNode child = (!root)? parent.CreateNewChild() : parent; child.DepOp = DepOps.GreaterThanEqual; }
	: LPAREN! GTE iexpr[child] RPAREN!
	;

gtexpr[DepNode parent, bool root]
{ DepNode child = (!root)? parent.CreateNewChild() : parent; child.DepOp = DepOps.GreaterThan; }
	: LPAREN! GT iexpr[child] RPAREN!
	;

ldexpr[DepNode parent, bool root]
{ DepNode child = (!root)? parent.CreateNewChild() : parent; child.DepOp = DepOps.Loaded; }
	: LPAREN! LD iexpr[child] RPAREN!
	;


iexpr[DepNode node]
{ node.Constraint = new DepConstraint(); }
	: c:CLASS ( v:VER { node.Constraint.VersionTmp=v.getText(); } )? { node.Constraint.Name=c.getText(); };
		
class DepLexer extends Lexer;

options
{
	k=2;
	charVocabulary='\u0000' .. '\u007F';
}

// Parentheses
LPAREN: '(' ;
RPAREN: ')' ;

// Combination Operators
NOTO: "!!" ;
AND: "&&" ;
OR: "||" ;
XOR: "^^" ;

// Dependency Operators
EQ: "==" ;
NEQ: "!=" ;
LTE: "<=" ;
LS: "<<" ;
GTE: ">=" ;
GT: ">>" ;
OPT: "??" ;
LD: "##" ;

// Version
protected
INT: ('0' .. '9')+;

VER: INT (DOT INT (DOT INT (DOT INT)?)?)?;

// Dot operator
protected
DOT: '.' ;

// Basic Identifier
protected
ID_START_LETTER: 
    ('a' .. 'z')
	| ('A' .. 'Z')
	| ('_')
	;

protected
ID_LETTER: 
  ID_START_LETTER
	| ('0' .. '9')
	;
	
protected
ID: ID_START_LETTER ( ID_LETTER )* ;

// Class
CLASS: ID ( DOT ID )* ;

// Whitespace
WS: ( ' '
    | '\r' '\n'
    | '\n'
    | '\t'
    )
    {$setType(Token.SKIP);}
    ;
