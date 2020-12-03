using Character.MapleLib.WzLib;
using Character.Core.Template;
using Character.Core.Util;

namespace Character.Core.GamePlay.Physics
{
    public class Foothold
    {
        public short Id { get; }

        public short Prev { get; }

        public short Next { get; }

        public short Layer { get; }

        public Range Horizontal { get; } = new Range();

        public Range Vertical { get; } = new Range();

        public short L => Horizontal.Smaller;

        public short R => Horizontal.Greater;

        public short T => Vertical.Smaller;

        public short B => Vertical.Greater;

        public short X1 => Horizontal.First;

        public short X2 => Horizontal.Second;

        public short Y1 => Vertical.First;

        public short Y2 => Vertical.Second;

        public bool IsWall => Id != 0 && Horizontal.Empty;

        public bool IsFloor => Id != 0 && Vertical.Empty;

        public bool IsLeftEdge => Id != 0 && Prev == 0;

        public bool IsRightEdge => Id != 0 && Next == 0;

        public bool HContains(short x) => Id != 0 && Horizontal.Contains(x);

        public bool VContains(short x) => Id != 0 && Vertical.Contains(x);

        public bool IsBlocking(Range vertical) => IsWall && Vertical.Overlaps(vertical);

        public short HDelta => Horizontal.Delta;

        public short VDelta => Vertical.Delta;

        public float Slope => IsWall ? 0f : (float) VDelta / HDelta;

        public float GroundBelow(float x) => IsFloor ? Y1 : Slope * (x - X1) + Y1;

        #region 构造函数

        public Foothold(WzObject src, short id, short layer)
        {
            Prev = (short) src["prev"];
            Next = (short) src["next"];
            Horizontal = new Range((short) src["x1"], (short) src["x2"]);
            Vertical = new Range((short) src["y1"], (short) src["y2"]);
            Id = id;
            Layer = layer;
        }

        public Foothold()
        {
            Id = 0;
            Layer = 0;
            Next = 0;
            Prev = 0;
        }

        #endregion

        /*
		// Returns the foothold id aka the identifier in game data of this platform.
		uint16_t id() const;
		// Returns the platform left to this.
		uint16_t prev() const;
		// Returns the platform right to this.
		uint16_t next() const;
		// Returns the platform's layer.
		uint8_t layer() const;
		// Returns the horizontal component.
		const Range<int16_t>& horizontal() const;
		// Returns the vertical component.
		const Range<int16_t>& vertical() const;

		// Return the left edge.
		int16_t l() const;
		// Return the right edge.
		int16_t r() const;
		// Return the top edge.
		int16_t t() const;
		// Return the bottom edge.
		int16_t b() const;
		// Return the first horizontal component.
		int16_t x1() const;
		// Return the second horizontal component.
		int16_t x2() const;
		// Return the first vertical component.
		int16_t y1() const;
		// Return the second vertical component.
		int16_t y2() const;
		// Return if the platform is a wall (x1 == x2).
		bool is_wall() const;
		// Return if the platform is a floor (y1 == y2).
		bool is_floor() const;
		// Return if this platform is a left edge.
		bool is_left_edge() const;
		// Return if this platform is a right edge.
		bool is_right_edge() const;
		// Returns if a x-coordinate is above or below this platform.
		bool hcontains(int16_t x) const;
		// Returns if a y-coordinate is right or left of this platform.
		bool vcontains(int16_t y) const;
		// Check whether this foothold blocks an object.
		bool is_blocking(const Range<int16_t>& vertical) const;
		// Returns the width.
		int16_t hdelta() const;
		// Returns the height.
		int16_t vdelta() const;
		// Returns the slope as a ratio of vertical/horizontal.
		double slope() const;
		// Returns a y-coordinate right below the given x-coordinate.
		double ground_below(double x) const;
         */
    }
}