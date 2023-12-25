using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : MonoBehaviour
{
	int[] positionArray = new int[64];	// where the pieces are. called positions in helper functions
	int[] validPosition = new int[64];	// valid moves for AuthScript. also called goodSpots
	int[] whiteAttacks = new int[64];	// squares white is attacking (for checkScript)
	int[] blackAttacks = new int[64];	// called blacksTargets in helper functions
	int startSquare;					// square picked up
	int targetSquare;					// the current selected square
	int turn;							// even is white, odd is black. also called player
	int enPassantSquare;				// value of jumped square, 99 otherwise. also frenchSquare
	int pickUpPutDown;					// 0 normally, 1 if just picked up, 2 if just put down
	int gameState;						// start screen (-1), main game (1), pawn promotion (2), game over (4+)
	int castles;						// explained in CheckCastling. also called castling
	int konami;							// easter egg, how many keys in a row have we got
	
	// smaller self-contained helper functions
    void Start()
    {
		startSquare = 0;
		targetSquare = 0;
		turn = 0;
		enPassantSquare = 99;
		pickUpPutDown = 0;
		gameState = -1;
		castles = 15;
		konami = 0;
		
		// just could not figure out how to initialise it with values set
		// so we make a reference one and copy it over
		int[] positions = new int[] {
		02,03,04,05,06,04,03,02,
		01,01,01,01,01,01,01,01,
		00,00,00,00,00,00,00,00,
		00,00,00,00,00,00,00,00,
		00,00,00,00,00,00,00,00,
		00,00,00,00,00,00,00,00,
		-1,-1,-1,-1,-1,-1,-1,-1,
		-2,-3,-4,-5,-6,-4,-3,-2
		};
		
		int i;
		for (i=0;i<64; i++)
		{
			positionArray[i] = positions[i];
		}
    }
	
	void Restart()
	{
		startSquare = 0;
		targetSquare = 0;
		turn = 0;
		enPassantSquare = 99;
		pickUpPutDown = 0;
		gameState = 1;
		castles = 15;
		konami = 0;
		
		int[] positions = new int[] {
		02,03,04,05,06,04,03,02,
		01,01,01,01,01,01,01,01,
		00,00,00,00,00,00,00,00,
		00,00,00,00,00,00,00,00,
		00,00,00,00,00,00,00,00,
		00,00,00,00,00,00,00,00,
		-1,-1,-1,-1,-1,-1,-1,-1,
		-2,-3,-4,-5,-6,-4,-3,-2
		};
		
		int i;
		for (i=0;i<64; i++)
		{
			positionArray[i] = positions[i];
		}
		
		FindSquaresAttacked(positionArray, whiteAttacks, blackAttacks, enPassantSquare);
	}
	
	public void PickUp()
	{
		pickUpPutDown = 1;
	}
	
	public void PutDown()
	{
		pickUpPutDown = 2;
	}
	
	public void PromotePawn(int number)
	{
		// rewrites the selected square in positionArray
		positionArray[targetSquare] = number;
	}
	
	public int GetGameState()
	{
		return gameState;
	}
	
	public void SetGameStateToOne()
	{
		gameState = 1;
	}
	
	public void AugmentTargetSquare(int number)
	{
		targetSquare += number;
	}
	
	public int GetTargetSquare()
	{
		return targetSquare;
	}
	
	public int GetValidPosition(int index)
	{
		return validPosition[index];
	}
	
	public int GetWhiteAttacks(int index)
	{
		return whiteAttacks[index];
	}
	
	public int GetBlackAttacks(int index)
	{
		return blackAttacks[index];
	}
	
	public int GetPositionArray(int index)
	{
		return positionArray[index];
	}
	
	double GetRand()
	{
		return ((double)(UnityEngine.Random.Range(0f, 1f)));
	}
	
	int GetIntRand()
	{
		return ((int)(UnityEngine.Random.Range(0f, 14414400f)));
	}
	
	int absint(int number)
	{
		// quicker to type and less cluttered
		// abs is a lowercase function in maths so the style guide violation is ok
		return Math.Abs(number);
	}
	
	// these functions help find the valid moves
	int FindNumberOfWhiteKings(int[] positions)
	{
		int count = 0;
		int i;
		for (i=0; i<64; i++)
		{
			if (positions[i] == 6)
			{
				count++;
			}
		}
		return count;
	}
	
	int FindNumberOfBlackKings(int[] positions)
	{
		int count = 0;
		int i;
		for (i=0; i<64; i++)
		{
			if (positions[i] == -6)
			{
				count++;
			}
		}
		return count;
	}
	
	void MovePiecesAround(int[] positions, int fromSquare, int toSquare)
	{
		// this function takes in a board set up, a from square and a to square,
		// and moves the pieces around. must be a valid move
		
		// before we move, we need to know if the to square is empty (for en passant)
		int target = positions[toSquare];	// zero if en passant is valid
		
		// write the new piece
		positions[toSquare] = positions[fromSquare];
		// erase the old piece
		positions[fromSquare] = 0;
		
		// if we castled (a king hopped two squares), move the rook
		if ((absint(positions[toSquare]) == 6) && (absint(fromSquare - toSquare) == 2))
		{
			// castle bottom left
			if ((toSquare==2))
			{
				// move the rook
				positions[0]=0;
				positions[3]=2;
			}
			// castle bottom right
			if (toSquare==6)
			{
				positions[7]=0;
				positions[5]=2;
			}
			// castle top left
			if (toSquare==58)
			{
				positions[56]=0;
				positions[59]=-2;
			}
			// castle top right
			if (toSquare==62)
			{
				positions[63]=0;
				positions[61]=-2;
			}
		}
		
		// when you move en passant you need to delete the pawn in a third square
		if ((absint(positions[toSquare]) == 1) && (absint(fromSquare-toSquare)%8 != 0) && (target == 0))
		{
			// it's a pawn that changed columns into an empty square
			// use the cell number to determine if you delete above or below the target
			if (toSquare > 26)
			{
				// top half of the board
				positions[toSquare-8] = 0;
			} else {
				// bottom half of the board
				positions[toSquare+8] = 0;
			}
		}
	}
	
	int FindWhiteKing(int[] positions)
	{
		int i;
		for (i=0; i<64; i++)
		{
			if (positions[i] == 6)
			{	
				return i;
			}
		}
		
		return 99;
	}
	
	int FindBlackKing(int[] positions)
	{
		int i;
		for (i=63; i>-1; i--)
		{
			// run it backwards so we get most potected king
			if (positions[i] == -6)
			{	
				return i;
			}
		}
		
		return 99;
	}
	
	bool OnBoard(int x, int y)
	{
		// check the xy coordinates are both within 0-7, inclusive
		return !((x < 0) || (y < 0) || (x > 7) || (y > 7));
	}
	
	void SearchInDirection(int[] positions, int[] goodSpots, int initialSquare, int deltaX, int deltaY)
	{
		// movement script for rooks and bishops. converts the index to an xy coordinate, deduces the colour,
		// and writes the valid squares for the direction to goodSpots
		
		// figure out if white (1) or black (-1)
		int whiteOrBlack;
		if (positions[initialSquare] == 0)
		{
			// avoid divide by zero error, also there's no piece to move
			return;
		} else {
			whiteOrBlack = positions[initialSquare]/absint(positions[initialSquare]);
		}
		
		// find coordinates
		int x = initialSquare%8;
		int y = (int)(initialSquare/8);
		int currentSquare;
		
		(x, y) = (x+deltaX, y+deltaY);
		while (OnBoard(x,y))
		{
			currentSquare = x + 8*y;
			
			// check not same colour
			if (positions[currentSquare]*whiteOrBlack < 1)
			{
				goodSpots[currentSquare] = 1;
			}
			
			// if empty, look at next square
			if (positions[currentSquare] == 0)
			{
				(x, y) = (x+deltaX, y+deltaY);
			} else {
				// otherwise we're done
				return;
			}
		}
	}
	
	// these functions write the valid moves
	void FindPawnMoves(int[] positions, int[] goodSpots, int initialSquare, int frenchSquare)
	{
		// writes all the valid spots for the pawn to move to with a 1 in goodSpots
		// note a pawn can never be on the back row (it will have promoted)
		
		// figure out if white or black
		int whiteOrBlack;
		if (positions[initialSquare] == 0)
		{
			return;
		} else {
			whiteOrBlack = positions[initialSquare]/absint(positions[initialSquare]);
		}
		
		// check not on promoting row
		if (initialSquare*whiteOrBlack > (int)(23.5f + 31.5f*(float)whiteOrBlack))
		{
			return;
		}
		
		// move one square in front
		if (positions[initialSquare+(8*whiteOrBlack)] == 0)
		{
			goodSpots[initialSquare+(8*whiteOrBlack)] = 1;
		}
		
		// move two squares in front if on start row and empty first square
		if (initialSquare*whiteOrBlack < (int)(-15.5f + 31.5f*(float)whiteOrBlack))
		{
			if ((positions[initialSquare+(8*whiteOrBlack)] == 0) && (positions[initialSquare+(16*whiteOrBlack)] == 0))
			{
				// nest ifs to avoid indexing out of bounds
				goodSpots[initialSquare+(16*whiteOrBlack)] = 1;
			}
		}
		
		// take diagonally left
		if (initialSquare%8 != 0)
		{
			if (positions[initialSquare - 1 + 8 * whiteOrBlack]*whiteOrBlack < 0)
			{
				goodSpots[initialSquare - 1 + 8 * whiteOrBlack] = 1;
			}
		}
		
		// take diagonally right
		if (initialSquare%8 != 7)
		{
			if ((positions[initialSquare + 1 + 8 * whiteOrBlack] * whiteOrBlack < 0))
			{
				goodSpots[initialSquare + 1 + 8 * whiteOrBlack] = 1;
			}
		}
		
		// en passant to the left
		if ((initialSquare %8 != 0) && (frenchSquare==initialSquare-1+8*whiteOrBlack) && (frenchSquare*whiteOrBlack > 24*whiteOrBlack))
		{
			goodSpots[initialSquare-1+8*whiteOrBlack] = 1;
		}
		
		// en passant to the right
		if ((initialSquare%8 != 7) && (frenchSquare==initialSquare+1+8*whiteOrBlack) && (frenchSquare*whiteOrBlack > 24*whiteOrBlack))
		{
			goodSpots[initialSquare+1+8*whiteOrBlack] = 1;
		}
	}
	
	void FindRookMoves(int[] positions, int[] goodSpots, int initialSquare)
	{
		// writes all the valid spots for the rook to move to with a 1 in goodSpots
		// can move up down left or right
		
		SearchInDirection(positions, goodSpots, initialSquare, 00, 01);	// up
		SearchInDirection(positions, goodSpots, initialSquare, 00, -1);	// down
		SearchInDirection(positions, goodSpots, initialSquare, -1, 00);	// left
		SearchInDirection(positions, goodSpots, initialSquare, 01, 00);	// right
	}
	
	void FindBishopMoves(int[] positions, int[] goodSpots, int initialSquare)
	{
		// just like the rook. moves diagonally
		
		SearchInDirection(positions, goodSpots, initialSquare, 01, 01);	// quadrant 1
		SearchInDirection(positions, goodSpots, initialSquare, -1, 01);	// quadrant 2
		SearchInDirection(positions, goodSpots, initialSquare, -1, -1);	// quadrant 3
		SearchInDirection(positions, goodSpots, initialSquare, 01, -1);	// quadrant 4
	}
	
	void FindKnightOrKingMoves(int[] positions, int[] goodSpots, int initialSquare)
	{
		// ChatGPT teaching me how to use foreach
		// writes the valid knight (or king) moves to goodSpots
		// to avoid circular dependencies we check for castling in another function (checkColourMove)
		
		// figure out if white or black
		int whiteOrBlack;
		if (positions[initialSquare] == 0)
		{
			// error checking
			return;
		} else {
			whiteOrBlack = positions[initialSquare]/absint(positions[initialSquare]);
		}
		
		int[] moves;
		int currentSquare, row_diff, col_diff;
		int piece = absint(positions[initialSquare]);
		if (piece == 3)
		{
			// knight
			moves = new int[] { -17, -15, -10, -6, 6, 10, 15, 17 };
		} else if (piece == 6)
		{
			// king
			moves = new int[] { -9, -8, -7, -1, 1, 7, 8, 9 };
		} else
		{
			return;
		}

		foreach (int move in moves)
		{
			currentSquare = initialSquare + move;
			if (currentSquare > -1 && currentSquare < 64) // check if the destination square is on the board
			{
				// check if the destination square is a valid knight move from the starting square AND doesn't hit a white piece
				row_diff = absint((int)(currentSquare/8) - (int)(initialSquare/8));
				col_diff = absint(currentSquare%8 - initialSquare%8);
				if ((piece == 3) && (row_diff + col_diff == 3) && (absint(row_diff - col_diff) == 1) && (positions[currentSquare]*whiteOrBlack < 1))
				{
					// knight
					goodSpots[currentSquare] = 1;
				}
				if ((piece == 6) && (row_diff < 2) && (col_diff < 2) && (positions[currentSquare]*whiteOrBlack < 1))
				{
					// king
					goodSpots[currentSquare] = 1;
				}
			}
		}
	}
	
	void FindSquaresWhiteAttacks(int[] positions, int[] whitesTargets, int initialSquare, int frenchSquare)
	{
		// this function is given a board, whitesTargets, an initial index, and info about the board
		// if the index is a white piece, it writes the squares it could move to into whitesTargets
		
		int piece = positions[initialSquare];
		
		switch (piece)
		{
			case 1: // white pawn
				FindPawnMoves(positions, whitesTargets, initialSquare, frenchSquare);
				break;
			case 2:	// white rook
				FindRookMoves(positions, whitesTargets, initialSquare);
				break;
			case 3: // white knight
				FindKnightOrKingMoves(positions, whitesTargets, initialSquare);
				break;
			case 4: // white bishop
				FindBishopMoves(positions, whitesTargets, initialSquare);
				break;
			case 5: // white queen, bishop + rook
				FindRookMoves(positions, whitesTargets, initialSquare);
				FindBishopMoves(positions, whitesTargets, initialSquare);
				break;
			case 6: // white king
				FindKnightOrKingMoves(positions, whitesTargets, initialSquare);
				break;
			default:
				// wasn't a white piece
				break;
		}
	}
	
	void FindSquaresBlackAttacks(int[] positions, int[] blacksTargets, int initialSquare, int frenchSquare)
	{
		// this function is given a board, blacksTargets, an initial index, and info about the board
		// if the index is a black piece, it writes the squares it could move to into blacksTargets
		
		int piece = positions[initialSquare];
		
		switch(piece)
		{
			case -1:	// black pawn
				FindPawnMoves(positions, blacksTargets, initialSquare, frenchSquare);
				break;
			case -2:	// black rook
				FindRookMoves(positions, blacksTargets, initialSquare);
				break;
			case -3:	// black knight
				FindKnightOrKingMoves(positions, blacksTargets, initialSquare);
				break;
			case -4:	// black bishop
				FindBishopMoves(positions, blacksTargets, initialSquare);
				break;
			case -5:	// black queen, bishop + rook
				FindRookMoves(positions, blacksTargets, initialSquare);
				FindBishopMoves(positions, blacksTargets, initialSquare);
				break;
			case -6:	// black king
				FindKnightOrKingMoves(positions, blacksTargets, initialSquare);
				break;
			default:
				// wasn't a black piece
				break;
		}
	}
	
	// these are other functions for pieces
	void FindAllSquaresWhiteAttacks(int[] positions, int[] whitesTargets, int frenchSquare)
	{
		// this function finds every square without an existing white piece where a black piece would be attacked
		int i;
		for (i=0;i<64;i++)
		{
			FindSquaresWhiteAttacks(positions, whitesTargets, i, frenchSquare);
			// since we don't reset whitesTargets, this builds up with every square
		}
	}
	
	void FindAllSquaresBlackAttacks(int[] positions, int[] blacksTargets, int frenchSquare)
	{
		// this function finds every square without an existing black piece where a white piece would be attacked
		int i;
		for (i=0;i<64;i++)
		{
			FindSquaresBlackAttacks(positions, blacksTargets, i, frenchSquare);
		}
	}

	bool IsTheWhiteKingInCheck(int[] positions, int frenchSquare)
	{
		// find the squares black attacks with a bespoke array
		int[] squaresBlackAttacks = new int[64];
		FindAllSquaresBlackAttacks(positions, squaresBlackAttacks, frenchSquare);
		
		// see if the white king is on one of those squares
		int index = FindWhiteKing(positions);
		if (index < 64)
		{
			if (squaresBlackAttacks[index] == 1)
			{
				return true;
			}
		}
		return false;
	}
	
	bool IsTheBlackKingInCheck(int[] positions, int frenchSquare)
	{
		// find the squares white attacks with a bespoke array
		int[] squaresWhiteAttacks = new int[64];
		FindAllSquaresWhiteAttacks(positions, squaresWhiteAttacks, frenchSquare);
		
		// see if the black king is on one of those squares
		int index = FindBlackKing(positions);
		if (index < 64)
		{
			if (squaresWhiteAttacks[index] == 1)
			{
				return true;
			}
		}
		return false;
	}
	
	// the big two
	void CheckWhiteMove(int[] positions, int[] safePositions, int initialSquare, int castling, int frenchSquare)
	{
		// this function is passed the board, an index (initialSquare), and some board info
		// and writes the valid positions that index could move to into safePositions (the squares attacked)
		
		// find the squares that piece attacks/checks (most of the work)
		FindSquaresWhiteAttacks(positions, safePositions, initialSquare, frenchSquare);
		
		// if it was a king, add in the relevant castling squares
		if (positions[initialSquare] == 6)
		{
			// make an array of squares black attacks and populate it
			int[] unsafes = new int[64];
			FindAllSquaresBlackAttacks(positions, unsafes, frenchSquare);
			
			if (((castling & 8) == 8) && positions[1] == 0 && positions[2] == 0 && positions[3] == 0 && (unsafes[2] + unsafes[3] + unsafes[4] == 0))
			{
				// castling bottom left is a valid move and the squares are free and not attacked
				safePositions[2] = 1;
			}
			if (((castling & 4) == 4) && positions[5] == 0 && positions[6] == 0 && (unsafes[4] + unsafes[5] + unsafes[6] == 0))
			{
				// castling bottom right is a valid move and the squares are free and not attacked
				safePositions[6] = 1;
			}
		}
		
		// remove the moves that would place your king in check (if you only have one king left)
		if (FindNumberOfWhiteKings(positions) == 1)
		{
			int i, destinationSquare;
			for (destinationSquare = 0; destinationSquare < 64; destinationSquare++)
			{
				if (safePositions[destinationSquare] == 1)
				{
					// play the move theoretically - build a fake position board and change that
					int[] fakePositionArray = new int[64];
					for (i=0; i<64; i++)
					{
						fakePositionArray[i] = positions[i];
					}
					
					MovePiecesAround(fakePositionArray, initialSquare, destinationSquare);
					
					// see if the king is now in check. if yes, remove that square from safePositions
					if (IsTheWhiteKingInCheck(fakePositionArray, frenchSquare))
					{
						safePositions[destinationSquare] = 0;
					}
				}
			}
		}
	}
	
	void CheckBlackMove(int[] positions, int[] safePositions, int initialSquare, int castling, int frenchSquare)
	{
		// this function is passed the board, an index, and some board info
		// and writes the valid positions that index could move to into safePositions (the squares attacked)
		
		// find the squares that piece attacks/checks (most of the work)
		FindSquaresBlackAttacks(positions, safePositions, initialSquare, frenchSquare);
		
		// if it was a king, add in the relevant castling squares
		if (positions[initialSquare] == -6)
		{
			// make an array of squares white attacks and populate it
			int[] unsafes = new int[64];
			FindAllSquaresWhiteAttacks(positions, unsafes, frenchSquare);
			
			if (((castling & 2) == 2) && positions[57] == 0 && positions[58] == 0 && positions[59] == 0 && (unsafes[58] + unsafes[59] + unsafes[60] == 0))
			{
				// castling bottom left is a valid move and the squares are free and not attacked
				safePositions[58] = 1;
			}
			if (((castling & 1) == 1) && positions[61] == 0 && positions[62] == 0 && (unsafes[60] + unsafes[61] + unsafes[62] == 0))
			{
				// castling bottom right is a valid move and the squares are free and not attacked
				safePositions[62] = 1;
			}
		}
		
		// remove the moves that would place your king in check (if you only have one king left)
		if (FindNumberOfBlackKings(positions) == 1)
		{
			int i, destinationSquare;
			for (destinationSquare = 0; destinationSquare < 64; destinationSquare++)
			{
				if (safePositions[destinationSquare] == 1)
				{
					// play the move theoretically - build a fake position board and change that
					int[] fakePositionArray = new int[64];
					for (i=0; i<64; i++)
					{
						fakePositionArray[i] = positions[i];
					}
					
					MovePiecesAround(fakePositionArray, initialSquare, destinationSquare);
					
					// see if the king is now in check. if yes, remove that square from safePositions
					if (IsTheBlackKingInCheck(fakePositionArray, frenchSquare))
					{
						safePositions[destinationSquare] = 0;
					}
				}
			}
		}
	}
	
	// other functions
	
	void FindSquaresAttacked(int[] positions, int[] squaresWhiteAttacks, int[] squaresBlackAttacks, int frenchSquare)
	{
		// this function writes the squares white and black attacks right now to two different arrays by reusing some functions
		
		// clear the given arrays
		Array.Clear(squaresWhiteAttacks, 0, 64);
		Array.Clear(squaresBlackAttacks, 0, 64);
		
		// and write to them
		FindAllSquaresWhiteAttacks(positions, squaresWhiteAttacks, frenchSquare);
		FindAllSquaresBlackAttacks(positions, squaresBlackAttacks, frenchSquare);
	}
	
	bool IsTheKingInCheck(int[] positions, int player, int frenchSquare)
	{
		// this function will take in a board state and the turn number (to figure out which King you're checking)
		// return true if the king is in check, and false if not
		
		// white is turn is divisible 2, black is not
		if (player%2 == 0)
		{
			// white's turn
			return IsTheWhiteKingInCheck(positions, frenchSquare);
		} else {
			return IsTheBlackKingInCheck(positions, frenchSquare);
		}
	}
	
	void CheckCastling(int[] positions, ref int castling)
	{
		// this function checks the pieces haven't moved. If they have then castling rights are lost
		// positions represents positionArray, castling represents castles
		
		// castles is an int that ranges from 0 to 15 (binary 0000 to 1111). 0 doesn't allow castling, 1 allows castling
		// The first bit refers to the bottom left, the second to the bottom right, third is top left, fourth is top right.
		// I implemented it this way to combine four variables into one, and also to challenge myself with bitwise operations
		
		// bottom left
		if ((positions[0] != 2) || (positions[4] != 6))
		{
			// the pieces have moved at some point
			castling = castling & 7;	// 0111
		}
		
		// bottom right
		if ((positions[7] != 2) || (positions[4] != 6))
		{
			// the pieces have moved at some point
			castling = castling & 11;	// 1011
		}
		
		// top left
		if ((positions[56] != -2) || (positions[60] != -6))
		{
			// the pieces have moved at some point
			castling = castling & 13;	// 1101
		}
		
		// top right
		if ((positions[63] != -2) || (positions[60] != -6))
		{
			// the pieces have moved at some point
			castling = castling & 14;	// 1110
		}
	}
	
	int CountNumberOfValidMoves(int[] positions, int player, int castling, int frenchSquare)
	{
		// is fed the board, the turn number (player), and some information about the board
		// and returns the number of valid moves that can be played in that position
		
		// initialise some things
		int i,j;
		int count = 0;
		int[] newValidPosition = new int[64];		
		
		for (i=0; i<64; i++)
		{
			// clear newValidPosition
			Array.Clear(newValidPosition, 0, 64);
			
			// fill it up again
			if (player%2 == 0)
			{
				// white's turn
				CheckWhiteMove(positions, newValidPosition, i, castling, frenchSquare);
			} else {
				CheckBlackMove(positions, newValidPosition, i, castling, frenchSquare);
			}
			
			// sum them all up
			for (j=0; j<64; j++)
			{
				count = count + newValidPosition[j];
			}
		}
		
		return count;
	}
	
	int FindGameState(int[] positions, int player, int castling, int frenchSquare)
	{
		// takes in the board, some info about it, and the turn/player (to see who to check)
		// returns the new game state, 6 is stalemate, 4 is black won, 5 is white won, 1 is normal play, 2 is promote
		
		// checkmate happens when your king is gone
		int whereTheWhiteKingIs=FindWhiteKing(positions);
		int whereTheBlackKingIs=FindBlackKing(positions);
		
		if (whereTheWhiteKingIs+whereTheBlackKingIs == 198)
		{
			// both gone
			return 6;
		}
		if (whereTheWhiteKingIs == 99)
		{
			// white lost
			return 4;
		}
		if (whereTheBlackKingIs == 99)
		{
			// black lost
			return 5;
		}
		
		// stalemate/classsic checkmate detection (if they can't move the game is over)
		int numValidMoves = CountNumberOfValidMoves(positions, player, castling, frenchSquare);
		bool isTheKingInCheck = IsTheKingInCheck(positions, player, frenchSquare);
		if ((numValidMoves == 0) && isTheKingInCheck)
		{
			// this is checkmate for the human player b/c otherwise we're not going to let them play a move
			// if white, black won (4). if black, white won (5)
			return (4 + (player%2));
		}
		if ((numValidMoves == 0) && (!isTheKingInCheck))
		{
			// stalemate
			return 6;
		}
		
		// are we promoting a pawn
		int i;
		for(i=0; i<8; i++)
		{
			if (positions[i] == -1)
			{
				return 2;
			}
		}
		
		for(i=56; i<64; i++)
		{
			if (positions[i] == 1)
			{
				return 2;
			}
		}
		
		// otherwise fine, as you were
		return 1;
	}
	
	// Update is called once per frame
    void Update()
    {
		// the player can only do two things, pick up and put down
		if (pickUpPutDown == 1)
		{
			// the player just picked something up
			pickUpPutDown = 0;
			
			// refresh variables
			startSquare = targetSquare;	// the from square when we make a move
			Array.Clear(validPosition, 0, 64);
			CheckCastling(positionArray, ref castles);
			
			// write to an array all the valid squares you can move to. this info is displayed by auth
			if (turn%2 == 0)
			{
				// white's turn
				CheckWhiteMove(positionArray, validPosition, startSquare, castles, enPassantSquare);
			} else {
				// black's turn
				CheckBlackMove(positionArray, validPosition, startSquare, castles, enPassantSquare);
			}
		}
		
		if (pickUpPutDown == 2)
		{
			// player just made a move
			pickUpPutDown = 0;
			
			// if it's a valid move then play it
			if ((validPosition[targetSquare] == 1))
			{
				// move the pieces around
				MovePiecesAround(positionArray, startSquare, targetSquare);
				
				// update enPassantVariable if we moved a pawn two squares
				// do this after moving pieces around b/c we need an accurate enPassantSquare variable for that
				if ((absint(positionArray[targetSquare]) == 1) && (absint(targetSquare - startSquare) == 16))
				{
					enPassantSquare = (targetSquare+startSquare)/2;
				} else {
					enPassantSquare=99;
				}
				
				// if it was a valid move it is now the other person's turn
				turn = turn+1;
				
				// determine the game state
				gameState = FindGameState(positionArray, turn, castles, enPassantSquare);
			}
			
			// clear out validPosition
			Array.Clear(validPosition, 0, 64);
		}
		
		// update the squares attacked arrays for the check script
		FindSquaresAttacked(positionArray, whiteAttacks, blackAttacks, enPassantSquare);
		
		// konami code - up up down down left right left right b a
		if (Input.anyKeyDown)
		{
			if (Input.GetKeyDown("up"))
			{
				if (konami < 2)
				{
					konami++;
				} else if (konami != 2)
				{
					konami = 0;
				}
			}
			else if (Input.GetKeyDown("down"))
			{
				if ((konami == 2) || (konami == 3))
				{
					konami++;
				} else {
					konami = 0;
				}
			}
			else if (Input.GetKeyDown("left"))
			{
				if ((konami == 4) || (konami == 6))
				{
					konami++;
				} else {
					konami = 0;
				}
			}
			else if (Input.GetKeyDown("right"))
			{
				if ((konami == 5) || (konami == 7))
				{
					konami++;
				} else {
					konami = 0;
				}
			}
			else if (Input.GetKeyDown("b"))
			{
				if (konami == 8)
				{
					konami++;
				} else {
					konami = 0;
				}
			}
			else if (Input.GetKeyDown("a"))
			{
				if (konami == 9)
				{
					konami++;
				} else {
					konami = 0;
				}
			}
			else
			{
				konami = 0;
			}
		}
		
		if (konami == 10)
		{
			gameState = 6;
			konami = 0;
		}
		
		// reset if you push r on the endscreen
		if ((gameState >= 4) && (Input.GetKeyDown("r")))
		{
			gameState=1;
			Restart();
		}
	}
}