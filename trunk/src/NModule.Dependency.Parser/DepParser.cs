// $ANTLR 2.7.5 (20050714): "nmodule-dep.g" -> "DepParser.cs"$

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

namespace NModule.Dependency.Parser
{
	// Generate the header common to all output files.
	using System;
	
	using TokenBuffer              = antlr.TokenBuffer;
	using TokenStreamException     = antlr.TokenStreamException;
	using TokenStreamIOException   = antlr.TokenStreamIOException;
	using ANTLRException           = antlr.ANTLRException;
	using LLkParser = antlr.LLkParser;
	using Token                    = antlr.Token;
	using IToken                   = antlr.IToken;
	using TokenStream              = antlr.TokenStream;
	using RecognitionException     = antlr.RecognitionException;
	using NoViableAltException     = antlr.NoViableAltException;
	using MismatchedTokenException = antlr.MismatchedTokenException;
	using SemanticException        = antlr.SemanticException;
	using ParserSharedInputState   = antlr.ParserSharedInputState;
	using BitSet                   = antlr.collections.impl.BitSet;
	
	public 	class DepParser : antlr.LLkParser
	{
		public const int EOF = 1;
		public const int NULL_TREE_LOOKAHEAD = 3;
		public const int LPAREN = 4;
		public const int AND = 5;
		public const int RPAREN = 6;
		public const int OR = 7;
		public const int XOR = 8;
		public const int OPT = 9;
		public const int EQ = 10;
		public const int NEQ = 11;
		public const int LTE = 12;
		public const int LS = 13;
		public const int GTE = 14;
		public const int GT = 15;
		public const int LD = 16;
		public const int NL = 17;
		public const int CLASS = 18;
		public const int VER = 19;
		public const int INT = 20;
		public const int DOT = 21;
		public const int ID_START_LETTER = 22;
		public const int ID_LETTER = 23;
		public const int ID = 24;
		public const int WS = 25;
		
		
		protected void initialize()
		{
			tokenNames = tokenNames_;
		}
		
		
		protected DepParser(TokenBuffer tokenBuf, int k) : base(tokenBuf, k)
		{
			initialize();
		}
		
		public DepParser(TokenBuffer tokenBuf) : this(tokenBuf,3)
		{
		}
		
		protected DepParser(TokenStream lexer, int k) : base(lexer,k)
		{
			initialize();
		}
		
		public DepParser(TokenStream lexer) : this(lexer,3)
		{
		}
		
		public DepParser(ParserSharedInputState state) : base(state,3)
		{
			initialize();
		}
		
	public void expr(
		DepNode root
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			cexpr(root);
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_0_);
		}
	}
	
	public void cexpr(
		DepNode parent
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			if ((LA(1)==LPAREN) && (LA(2)==AND))
			{
				andexpr(parent);
			}
			else if ((LA(1)==LPAREN) && (LA(2)==OR)) {
				orexpr(parent);
			}
			else if ((LA(1)==LPAREN) && (LA(2)==XOR)) {
				xorexpr(parent);
			}
			else if ((LA(1)==LPAREN) && (LA(2)==OPT)) {
				optexpr(parent);
			}
			else if ((LA(1)==LPAREN) && ((LA(2) >= EQ && LA(2) <= NL))) {
				oexpr(parent, true);
			}
			else
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_1_);
		}
	}
	
	public void andexpr(
		DepNode parent
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			match(LPAREN);
			match(AND);
			parent.DepOp = DepOps.And;
			{ // ( ... )+
				int _cnt6=0;
				for (;;)
				{
					if ((LA(1)==LPAREN) && ((LA(2) >= EQ && LA(2) <= NL)) && (LA(3)==CLASS))
					{
						oexpr(parent, false);
					}
					else if ((LA(1)==LPAREN) && (tokenSet_2_.member(LA(2))) && (LA(3)==LPAREN||LA(3)==CLASS)) {
						{
							DepNode child = parent.CreateNewChild();
							cexpr(child);
						}
					}
					else
					{
						if (_cnt6 >= 1) { goto _loop6_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
					}
					
					_cnt6++;
				}
_loop6_breakloop:				;
			}    // ( ... )+
			match(RPAREN);
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_1_);
		}
	}
	
	public void orexpr(
		DepNode parent
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			match(LPAREN);
			match(OR);
			parent.DepOp = DepOps.Or;
			{ // ( ... )+
				int _cnt10=0;
				for (;;)
				{
					if ((LA(1)==LPAREN) && ((LA(2) >= EQ && LA(2) <= NL)) && (LA(3)==CLASS))
					{
						oexpr(parent, false);
					}
					else if ((LA(1)==LPAREN) && (tokenSet_2_.member(LA(2))) && (LA(3)==LPAREN||LA(3)==CLASS)) {
						{
							DepNode child = parent.CreateNewChild();
							cexpr(child);
						}
					}
					else
					{
						if (_cnt10 >= 1) { goto _loop10_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
					}
					
					_cnt10++;
				}
_loop10_breakloop:				;
			}    // ( ... )+
			match(RPAREN);
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_1_);
		}
	}
	
	public void xorexpr(
		DepNode parent
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			match(LPAREN);
			match(XOR);
			parent.DepOp = DepOps.Xor;
			{ // ( ... )+
				int _cnt14=0;
				for (;;)
				{
					if ((LA(1)==LPAREN) && ((LA(2) >= EQ && LA(2) <= NL)) && (LA(3)==CLASS))
					{
						oexpr(parent, false);
					}
					else if ((LA(1)==LPAREN) && (tokenSet_2_.member(LA(2))) && (LA(3)==LPAREN||LA(3)==CLASS)) {
						{
							DepNode child = parent.CreateNewChild();
							cexpr(child);
						}
					}
					else
					{
						if (_cnt14 >= 1) { goto _loop14_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
					}
					
					_cnt14++;
				}
_loop14_breakloop:				;
			}    // ( ... )+
			match(RPAREN);
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_1_);
		}
	}
	
	public void optexpr(
		DepNode parent
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			match(LPAREN);
			match(OPT);
			parent.DepOp = DepOps.Opt;
			{ // ( ... )+
				int _cnt18=0;
				for (;;)
				{
					if ((LA(1)==LPAREN) && ((LA(2) >= EQ && LA(2) <= NL)) && (LA(3)==CLASS))
					{
						oexpr(parent, false);
					}
					else if ((LA(1)==LPAREN) && (tokenSet_2_.member(LA(2))) && (LA(3)==LPAREN||LA(3)==CLASS)) {
						{
							DepNode child = parent.CreateNewChild();
							cexpr(child);
						}
					}
					else
					{
						if (_cnt18 >= 1) { goto _loop18_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
					}
					
					_cnt18++;
				}
_loop18_breakloop:				;
			}    // ( ... )+
			match(RPAREN);
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_1_);
		}
	}
	
	public void oexpr(
		DepNode parent, bool root
	) //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			if ((LA(1)==LPAREN) && (LA(2)==EQ))
			{
				eqexpr(parent, root);
			}
			else if ((LA(1)==LPAREN) && (LA(2)==NEQ)) {
				neqexpr(parent, root);
			}
			else if ((LA(1)==LPAREN) && (LA(2)==LTE)) {
				lteexpr(parent, root);
			}
			else if ((LA(1)==LPAREN) && (LA(2)==LS)) {
				ltexpr(parent, root);
			}
			else if ((LA(1)==LPAREN) && (LA(2)==GTE)) {
				gteexpr(parent, root);
			}
			else if ((LA(1)==LPAREN) && (LA(2)==GT)) {
				gtexpr(parent, root);
			}
			else if ((LA(1)==LPAREN) && (LA(2)==LD)) {
				ldexpr(parent, root);
			}
			else if ((LA(1)==LPAREN) && (LA(2)==NL)) {
				nlexpr(parent, root);
			}
			else
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_1_);
		}
	}
	
	public void eqexpr(
		DepNode parent, bool root
	) //throws RecognitionException, TokenStreamException
{
		
		DepNode child = (!root)? parent.CreateNewChild() : parent; child.DepOp = DepOps.Equal;
		
		try {      // for error handling
			match(LPAREN);
			match(EQ);
			iexpr(child);
			match(RPAREN);
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_1_);
		}
	}
	
	public void neqexpr(
		DepNode parent, bool root
	) //throws RecognitionException, TokenStreamException
{
		
		DepNode child = (!root)? parent.CreateNewChild() : parent; child.DepOp = DepOps.NotEqual;
		
		try {      // for error handling
			match(LPAREN);
			match(NEQ);
			iexpr(child);
			match(RPAREN);
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_1_);
		}
	}
	
	public void lteexpr(
		DepNode parent, bool root
	) //throws RecognitionException, TokenStreamException
{
		
		DepNode child = (!root)? parent.CreateNewChild() : parent; child.DepOp = DepOps.LessThanEqual;
		
		try {      // for error handling
			match(LPAREN);
			match(LTE);
			iexpr(child);
			match(RPAREN);
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_1_);
		}
	}
	
	public void ltexpr(
		DepNode parent, bool root
	) //throws RecognitionException, TokenStreamException
{
		
		DepNode child = (!root)? parent.CreateNewChild() : parent; child.DepOp = DepOps.LessThan;
		
		try {      // for error handling
			match(LPAREN);
			match(LS);
			iexpr(child);
			match(RPAREN);
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_1_);
		}
	}
	
	public void gteexpr(
		DepNode parent, bool root
	) //throws RecognitionException, TokenStreamException
{
		
		DepNode child = (!root)? parent.CreateNewChild() : parent; child.DepOp = DepOps.GreaterThanEqual;
		
		try {      // for error handling
			match(LPAREN);
			match(GTE);
			iexpr(child);
			match(RPAREN);
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_1_);
		}
	}
	
	public void gtexpr(
		DepNode parent, bool root
	) //throws RecognitionException, TokenStreamException
{
		
		DepNode child = (!root)? parent.CreateNewChild() : parent; child.DepOp = DepOps.GreaterThan;
		
		try {      // for error handling
			match(LPAREN);
			match(GT);
			iexpr(child);
			match(RPAREN);
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_1_);
		}
	}
	
	public void ldexpr(
		DepNode parent, bool root
	) //throws RecognitionException, TokenStreamException
{
		
		DepNode child = (!root)? parent.CreateNewChild() : parent; child.DepOp = DepOps.Loaded;
		
		try {      // for error handling
			match(LPAREN);
			match(LD);
			iexpr(child);
			match(RPAREN);
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_1_);
		}
	}
	
	public void nlexpr(
		DepNode parent, bool root
	) //throws RecognitionException, TokenStreamException
{
		
		DepNode child = (!root)? parent.CreateNewChild() : parent; child.DepOp = DepOps.NotLoaded;
		
		try {      // for error handling
			match(LPAREN);
			match(NL);
			iexpr(child);
			match(RPAREN);
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_1_);
		}
	}
	
	public void iexpr(
		DepNode node
	) //throws RecognitionException, TokenStreamException
{
		
		IToken  c = null;
		IToken  v = null;
		node.Constraint = new DepConstraint();
		
		try {      // for error handling
			c = LT(1);
			match(CLASS);
			{
				switch ( LA(1) )
				{
				case VER:
				{
					v = LT(1);
					match(VER);
					node.Constraint.SetVersion (v.getText());
					break;
				}
				case RPAREN:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			node.Constraint.Name=c.getText();
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_3_);
		}
	}
	
	private void initializeFactory()
	{
	}
	
	public static readonly string[] tokenNames_ = new string[] {
		@"""<0>""",
		@"""EOF""",
		@"""<2>""",
		@"""NULL_TREE_LOOKAHEAD""",
		@"""LPAREN""",
		@"""AND""",
		@"""RPAREN""",
		@"""OR""",
		@"""XOR""",
		@"""OPT""",
		@"""EQ""",
		@"""NEQ""",
		@"""LTE""",
		@"""LS""",
		@"""GTE""",
		@"""GT""",
		@"""LD""",
		@"""NL""",
		@"""CLASS""",
		@"""VER""",
		@"""INT""",
		@"""DOT""",
		@"""ID_START_LETTER""",
		@"""ID_LETTER""",
		@"""ID""",
		@"""WS"""
	};
	
	private static long[] mk_tokenSet_0_()
	{
		long[] data = { 2L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_0_ = new BitSet(mk_tokenSet_0_());
	private static long[] mk_tokenSet_1_()
	{
		long[] data = { 82L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_1_ = new BitSet(mk_tokenSet_1_());
	private static long[] mk_tokenSet_2_()
	{
		long[] data = { 262048L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_2_ = new BitSet(mk_tokenSet_2_());
	private static long[] mk_tokenSet_3_()
	{
		long[] data = { 64L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_3_ = new BitSet(mk_tokenSet_3_());
	
}
}
