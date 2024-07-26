public class Node {
    public int Id { get; private set; }
    public RoomType RoomType { get; private set; }

    public static bool operator ==(Node t_left, Node t_right) {
        if (ReferenceEquals(t_left, t_right)) {
            return true;
        } // Checks if both objects are the same instance.
        if (ReferenceEquals(t_left, null) || ReferenceEquals(t_right, null)) {
            return false;
        } // Checks if either object is null.
        return t_left.RoomType == t_right.RoomType;
    }

    public static bool operator !=(Node t_left, Node t_right) {
        return !(t_left == t_right);
    }

    public class Builder {
        private int m_id = 0;
        private RoomType m_roomType = RoomType.None;


        public Builder withId(int t_id) {
            m_id = t_id;
            return this;
        }

        public Builder withRoomType(string t_roomType) {
            switch (t_roomType) {
                case "Goal":
                    m_roomType = RoomType.Goal;
                    break;
                case "Entrance":
                    m_roomType = RoomType.Entrance;
                    break;
                case "Room":
                    m_roomType = RoomType.Room;
                    break;
                default:
                    throw new Exception($"{t_roomType} is not recognized as a room type");
                    break;
            }
            return this;
        }

        public Node build() {
            return new Node {
                Id = m_id,
                RoomType = m_roomType
            };
            ;
        }
    }
}

public enum RoomType {
    None = 0,
    Entrance = 1,
    Goal = 2,
    Room = 3,
}