public static class DirrectionsExt
{
	public static EQuadrants GetQuadrant(this EDirections d)
	{
		return (EQuadrants)d;
	}

	public static bool isStickQuadrant(this EQuadrants quadrant)
	{
		return quadrant >= EQuadrants.One && quadrant <= EQuadrants.Eight;
	}
}
