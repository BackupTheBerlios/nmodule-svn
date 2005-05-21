options
{
	language="CSharp"; 
	namespace="NModule.Dependency.Parser";
}
  
class DepParser extends Parser;

options {
	buildAST=true;
	k=3;
}

expr: 
	cexpr
	;
	
cexpr:
    LPAREN! ((NOTO^|AND^|OR^|XOR^|OPT^) (oexpr|cexpr)+) RPAREN!
    | oexpr
    ;

oexpr:
	LPAREN! ((EQ^|NEQ^|LTE^|LS^|GTE^|GT^|LD^) iexpr) RPAREN!
	;
	
iexpr: CLASS ( VER )?;

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

VER: INT (DOT INT)*;

// Dot operator
protected
DOT: '.' ;

// Basic Identifier
protected
ID_START_LETTER: 
    ('a' .. 'z')
	| ('A' .. 'Z')
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
