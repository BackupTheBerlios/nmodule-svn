//
// nmodule-dep.g
//
// Author:
//     Michael Tindal <urilith@gentoo.org>
//
// Copyright (C) 2005 Michael Tindal and the individuals listed on
// the ChangeLog entries.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

header
{
//
// ANTLR Generated Files.
//
// Author:
//     Michael Tindal <urilith@gentoo.org>
//
// Copyright (C) 2005 Michael Tindal and the individuals listed on
// the ChangeLog entries.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
 
using NModule.Dependency.Core;
}

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
	: cexpr[root]
	;

// LPAREN! ((NOTO^|AND^|OR^|XOR^|OPT^) (oexpr|cexpr)+) RPAREN!
cexpr[DepNode parent]
	: notexpr[parent]
	| andexpr[parent]
	| orexpr[parent]
	| xorexpr[parent]
	| optexpr[parent]
	| oexpr[parent, true]
	;

notexpr[DepNode parent]
	: LPAREN! NOTO (oexpr[parent, false]|({DepNode child = parent.CreateNewChild(); } cexpr[child]))+ RPAREN!
	;

andexpr[DepNode parent]
	: LPAREN! AND (oexpr[parent, false]|({DepNode child = parent.CreateNewChild(); } cexpr[child]))+ RPAREN!
	;

orexpr[DepNode parent]
	: LPAREN! OR (oexpr[parent, false]|({DepNode child = parent.CreateNewChild(); } cexpr[child]))+ RPAREN!
	;

xorexpr[DepNode parent]
	: LPAREN! XOR (oexpr[parent, false]|({DepNode child = parent.CreateNewChild(); } cexpr[child]))+ RPAREN!
	;

optexpr[DepNode parent]
	: LPAREN! OPT (oexpr[parent, false]|({DepNode child = parent.CreateNewChild(); } cexpr[child]))+ RPAREN!
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
ID_START_LETTER 
	: ('a' .. 'z')
	| ('A' .. 'Z')
	| ('_')
	;

protected
ID_LETTER
	: ID_START_LETTER
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
