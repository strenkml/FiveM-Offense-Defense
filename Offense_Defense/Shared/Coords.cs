namespace OffenseDefense.Client {
    class Coords {
        private static float HeadingToAngle(float heading) {
            float angle = heading + 90;
            if (angle >= 360) {
                return angle - 360;
            }
            return angle;
        }

        private static float AngleToTriangle(float angle) {
            return angle % 90;
        }
        
        private static int GetQuadrant(float angle) {
            if (angle > 0 && angle < 90) {
                return 1;
            }

            if (angle > 90 && angle < 180) {
                return 2;
            }

            if (angle > 180 && angle < 270) {
                return 3;
            }

            if (angle > 270 && angle < 360) {
                return 4;
            }

            return -1;
        }

        private static bool RequiresComplexComputations(float heading) {
            float remainder = heading % 90;
            if (remainder == 0) {
                return false;
            }
            return true;
        }

        private static void FindOffsets(float heading, float distance, out float a, out float b) {
            float angle = HeadingToAngle(heading);
            float triangle = AngleToTringle(angle);

            a = distance * (Math.Sin(triangle));
            b = sqrt((distance^2) - (a^2));
        }

        public static MapMarker GetFront(MapMarker coord, float distance) {
            float heading = coord.heading;
            float x = coord.x;
            float y = coord.y;
            float z = coord.z;

            float shiftX = 0;
            float shiftY = 0;

            float newX = 0;
            float newY = 0;

            float angle = HeadingToAngle(heading);
            if (RequiresComplexComputations(heading)) {
                float a;
                float b;

                FindOffsets(heading, distance, out a, out b);

                int quadrant = GetQuadrant(angle);

                switch(quadrant) {
                    case 1:
                        shiftX = b;
                        shiftY = a;
                        break;
                    case 2:
                        shiftX = -a;
                        shiftY = -b;
                        break;
                    case 3:
                        shiftX = -b;
                        shiftY = -a;
                        break;
                    case 4:
                        shiftX = a;
                        shiftY = -b;
                        break;
                }
            } else {
                switch(angle) {
                    case 0:
                        shiftX = distance;
                        break;
                    case 90:
                        shiftY = distance;
                        break;
                    case 180:
                        shiftX = -distance;
                        break;
                    case 270:
                        shiftY = -distance;
                        break;
                }
            }

            newX = x + shiftX;
            newY = y + shiftY;

            return new MapMarker(newX, newY, z, heading);
        }

        public static MapMarker GetBehind(MapMarker coord, float distance) {
            float heading = coord.heading;
            float x = coord.x;
            float y = coord.y;
            float z = coord.z;

            float shiftX = 0;
            float shiftY = 0;

            float newX = 0;
            float newY = 0;

            float angle = HeadingToAngle(heading);

            if (RequiresComplexComputations(heading)) {
                float a;
                float b;

                FindOffsets(heading, distance, out a, out b);

                int quadrant = GetQuadrant(angle);

                switch(quadrant) {
                    case 1:
                        shiftX = -b;
                        shiftY = -a;
                        break;
                    case 2:
                        shiftX = a;
                        shiftY = -b;
                        break;
                    case 3:
                        shiftX = b;
                        shiftY = a;
                        break;
                    case 4:
                        shiftX = -a;
                        shiftY = b;
                        break;
                }
            } else {
                switch(angle) {
                    case 0:
                        shiftX = -distance;
                        break;
                    case 90:
                        shiftY = -distance;
                        break;
                    case 180:
                        shiftX = distance;
                        break;
                    case 270:
                        shiftY = distance;
                        break;
                }
            }

            newX = x + shiftX;
            newY = y + shiftY;

            return new MapMarker(newX, newY, z, heading);
        }

        public static MapMarker GetLeft(MapMarker coord, float distance) {
            float heading = coord.heading;
            float x = coord.x;
            float y = coord.y;
            float z = coord.z;

            float shiftX = 0;
            float shiftY = 0;

            float newX = 0;
            float newY = 0;

            float angle = HeadingToAngle(heading);

            if (RequiresComplexComputations(heading)) {
                float a;
                float b;

                FindOffsets(heading, distance, out a, out b);

                int quadrant = GetQuadrant(angle);

                switch(quadrant) {
                    case 1:
                        shiftX = -a;
                        shiftY = b;
                        break;
                    case 2:
                        shiftX = -b;
                        shiftY = -a;
                        break;
                    case 3:
                        shiftX = b;
                        shiftY = -a;
                        break;
                    case 4:
                        shiftX = b;
                        shiftY = a;
                        break;
                }
            } else {
                switch(angle) {
                    case 0:
                        shiftY = distance;
                        break;
                    case 90:
                        shiftX = -distance;
                        break;
                    case 180:
                        shiftY = -distance;
                        break;
                    case 270:
                        shiftX = distance;
                        break;
                }
            }

            newX = x + shiftX;
            newY = y + shiftY;

            return new MapMarker(newX, newY, z, heading);
        }

        public static MapMarker GetRight(MapMarker coord, float distance) {
            float heading = coord.heading;
            float x = coord.x;
            float y = coord.y;
            float z = coord.z;

            float shiftX = 0;
            float shiftY = 0;

            float newX = 0;
            float newY = 0;

            float angle = HeadingToAngle(heading);

            if (RequiresComplexComputations(heading)) {
                float a;
                float b;

                FindOffsets(heading, distance, out a, out b);

                int quadrant = GetQuadrant(angle);

                switch(quadrant) {
                    case 1:
                        shiftX = a;
                        shiftY = -b;
                        break;
                    case 2:
                        shiftX = b;
                        shiftY = a;
                        break;
                    case 3:
                        shiftX = -b;
                        shiftY = a;
                        break;
                    case 4:
                        shiftX = -b;
                        shiftY = -a;
                        break;
                }
            } else {
                switch(angle) {
                    case 0:
                        shiftY = -distance;
                        break;
                    case 90:
                        shiftX = distance;
                        break;
                    case 180:
                        shiftY = distance;
                        break;
                    case 270:
                        shiftX = -distance;
                        break;
                }
            }

            newX = x + shiftX;
            newY = y + shiftY;

            return new MapMarker(newX, newY, z, heading);
        }
    }
}