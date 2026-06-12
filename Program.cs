namespace KarataConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("1- Given a list of (employee, action) records where action is \"enter\" or \"exit\" find: (a) employees who entered without exiting, (b) employees who exited without entering.");
                var records1 = new List<(string employee, string action)>{
                    ("Paul",    "enter"),
                    ("Pauline", "exit"),    // exit without enter
                    ("Paul",    "enter"),   // enter while already inside
                    ("Paul",    "exit"),
                    ("Martha",  "exit"),    // exit without enter
                    ("Joe",     "enter"),
                    ("Martha",  "enter"),
                    ("Steve",   "enter"),
                    ("Martha",  "exit"),
                    ("Jennifer","enter"),
                    ("Joe",     "enter"),   // enter while already inside
                    ("Curtis",  "exit"),    // exit without enter
                    ("Curtis",  "enter"),
                    ("Joe",     "exit"),
                    ("Martha",  "enter"),
                    ("Martha",  "exit"),
                    ("Jennifer","exit"),
                    ("Joe",     "enter"),
                    ("Joe",     "exit"),
                    ("Bill",    "enter")   // still inside at end
                };

                var inside = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var entredWithoutExiting = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var exitedWithoutEntering = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                records1.ForEach(record =>
                {
                   if(record.action.Equals("enter") && !inside.Add(record.employee))
                    {
                        entredWithoutExiting.Add(record.employee);
                    }
                    else if(record.action.Equals("exit") && !inside.Remove(record.employee))
                    {
                        exitedWithoutEntering.Add(record.employee);
                    }
                    // else
                    // {
                    //     throw new InvalidOperationException($"Invalid action {record.action} for employee {record.employee}");
                    // }
                });

                entredWithoutExiting.UnionWith(inside);

                Console.WriteLine("Employees who entered without exiting: " + string.Join(", ", entredWithoutExiting));

                Console.WriteLine("Employees who exited without entering: " + string.Join(", ", exitedWithoutEntering));


            Console.WriteLine("===========================================================================================");
            // Console.ReadLine();



            Console.WriteLine("2- Given (name, time \"HHMM\") records for one day, find employees who badged in **3+ times in any 1-hour window**, returning the times in that window.");

            var records2 = new List<(string employee, string time)>{
                ("Paul",    "1315"),
                ("Jennifer","1910"),
                ("John",    "0830"),
                ("Paul",    "1355"),
                ("John",    "0915"),
                ("John",    "0900"),
                ("Josh",    "2200"),
                ("John",    "1600"),
                ("Marta",   "1600"),
                ("Josh",    "2245"),
                ("Jennifer","1335"),
                ("Jennifer","1305"),
                ("Paul",    "1405"),
                ("Jennifer","1230"),
                ("Josh",    "2330")
            };

           var byEmployee = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            records2.ForEach(record =>
            {
                if(byEmployee.ContainsKey(record.employee))
                {
                    byEmployee[record.employee].Add(record.time);
                }
                else
                {
                    byEmployee[record.employee] = new List<string>{ record.time };
                }
            });

            var result = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            foreach(var kvp in byEmployee)
            {
                var times = kvp.Value.OrderBy(t => t).ToList();

                for(int i = 0; i < times.Count - 2; i++)
                {
                    var start = DateTime.ParseExact(times[i].ToString(), "HHmm", null);
                    var end = DateTime.ParseExact(times[i + 2].ToString(), "HHmm", null);
                    
                    if((end - start).TotalMinutes <= 60)
                    {
                        if(!result.ContainsKey(kvp.Key))
                        {
                            result[kvp.Key] = new List<string>();
                        }
                        result[kvp.Key].Add($"{times[i]}, {times[i + 1]}, {times[i + 2]}");
                    }
                }
            }

             foreach (var (name, times) in result)
                Console.WriteLine($"{name}: {string.Join(", ", times)}");
            Console.WriteLine("===========================================================================================");



             Console.WriteLine("3- Records of (employee, room, enter/exit time). Find the room used by the most unique people, and total occupancy time per room.");

             var records3 = new List<BadgeRecord>
            {
                new("Paul",     "RoomA", "enter", "0900"),
                new("Jennifer", "RoomA", "enter", "0905"),
                new("Paul",     "RoomA", "exit",  "0930"),
                new("Paul",     "RoomB", "enter", "0935"),
                new("Jennifer", "RoomA", "exit",  "1000"),
                new("John",     "RoomB", "enter", "1000"),
                new("Paul",     "RoomB", "exit",  "1015"),
                new("John",     "RoomB", "exit",  "1100"),
                new("Marta",    "RoomA", "enter", "1100"),
                new("Marta",    "RoomA", "exit",  "1130"),
                new("John",     "RoomC", "enter", "1200"),
                new("John",     "RoomC", "exit",  "1230"),
            };

            var roomUniqueUsers = new Dictionary<string, HashSet<string>>();
            var roomTotalTimes = new Dictionary<string, double>();
            var activeSessions = new Dictionary<string, string>(); // Key: "Employee-Room", Value: EnterTime

            records3.ForEach(record =>
            {
                if(!roomUniqueUsers.ContainsKey(record.Room))
                {
                    roomUniqueUsers[record.Room] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                }
                roomUniqueUsers[record.Room].Add(record.Employee);

                var sessionKey = $"{record.Employee}-{record.Room}";

                if(record.Action.Equals("enter", StringComparison.OrdinalIgnoreCase))
                {
                    activeSessions[sessionKey] = record.Time;
                }
                else if(record.Action.Equals("exit", StringComparison.OrdinalIgnoreCase) && activeSessions.ContainsKey(sessionKey))
                {
                    var enterTime = DateTime.ParseExact(activeSessions[sessionKey], "HHmm", null);
                    var exitTime = DateTime.ParseExact(record.Time, "HHmm", null);
                    var duration = (exitTime - enterTime).TotalMinutes;

                    if(!roomTotalTimes.ContainsKey(record.Room))
                    {
                        roomTotalTimes[record.Room] = 0;
                    }
                    roomTotalTimes[record.Room] = roomTotalTimes[record.Room] + duration;

                    activeSessions.Remove(sessionKey);
                }
            });

            var mostPopularRoom = roomUniqueUsers
                .OrderByDescending(room => room.Value.Count)
                .FirstOrDefault();

            Console.WriteLine($"Room with most unique people: {mostPopularRoom.Key} ({mostPopularRoom.Value.Count} unique visitors)\n");  
            Console.WriteLine("Total occupancy time per room:");
            foreach (var room in roomTotalTimes)
            {
                Console.WriteLine($"- {room.Key}: {room.Value} minutes");
            }  

           Console.WriteLine("===========================================================================================");

           Console.WriteLine("4 - Given enter/exit timestamp pairs, find the time when the most people were in the office at once.");

            var intervals = new List<(string Enter, string Exit)>
            {
                ("0900", "1130"),
                ("0915", "1000"),
                ("0930", "1015"),
                ("1000", "1200"),
                ("1045", "1100"),
                ("1300", "1400"),
            };

            var events = new List<(DateTime Time, int Change)>();

            intervals.ForEach(interval =>
            {
                var enterTime = DateTime.ParseExact(interval.Enter, "HHmm", null);
                var exitTime = DateTime.ParseExact(interval.Exit, "HHmm", null);

                events.Add((enterTime, 1)); // +1 for enter
                events.Add((exitTime, -1));  // -1 for exit
            });

            events = events.OrderBy(e => e.Time).ThenBy(e => e.Change).ToList();

            int currentCount = 0;
            int maxCount = 0;
            var peakStartTime = new DateTime();
            var peakEndTime = new DateTime();
            var k = 0;
            foreach(var e in events)
            {
                currentCount += e.Change;
                if(currentCount > maxCount)
                {
                    maxCount = currentCount;
                    peakStartTime =  e.Time;
                    peakEndTime = k + 1 < events.Count ? events[k + 1].Time : events[k].Time;
                }

                k++;
            }

            Console.WriteLine($"Peak occupancy: {maxCount} people");
            Console.WriteLine($"From {peakStartTime} to {peakEndTime}");

            Console.WriteLine("===========================================================================================");

            Console.WriteLine("5 - **Subdomain visit counts**");

             var input = new[]
            {
                "900,google.com",
                "60,mail.yahoo.com",
                "10,mobile.sports.yahoo.com",
                "40,sports.yahoo.com",
                "300,yahoo.com",
                "10,stackoverflow.com",
                "20,overflow.com",
                "5,com.com",
                "1,internal.intranet.local",
            };
 
            var domainCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach(var entry in input)
            {
                var parts = entry.Split(',');
                var count = int.Parse(parts[0]);
                var domain = parts[1];

                var subdomains = domain.Split('.');

                for(int i = 0; i < subdomains.Length; i++)
                {
                    var subdomain = string.Join('.', subdomains.Skip(i));
                    if(domainCounts.ContainsKey(subdomain))
                    {
                        domainCounts[subdomain] += count;
                    }
                    else
                    {
                        domainCounts[subdomain] = count;
                    }
                }
            };

            var counts = domainCounts.OrderByDescending(kv => kv.Value).ThenBy(kv => kv.Key).ToList();
             
            foreach (var (domain, count) in counts)
                Console.WriteLine($"{count,6}  {domain}");


            Console.WriteLine("===========================================================================================");

            Console.WriteLine("6 - Two lists: completed purchases `[\"userId,time,amount\"]` and ad clicks `[\"ip,time,adText\"]` plus a user→ip map. For each ad text, output \"purchases / clicks\".");

              var purchases = new[]
            {
                "u1,1000,99.99",
                "u2,1030,25.00",
                "u1,1100,49.50",
                "u5,1200,15.75",
            };
 
            var clicks = new[]
            {
                "10.0.0.1,0950,Buy wool sweaters!",
                "10.0.0.2,0955,Buy wool sweaters!",
                "10.0.0.3,1005,50% off boots",
                "10.0.0.1,1010,50% off boots",
                "10.0.0.4,1015,50% off boots",
                "10.0.0.9,1020,Free shipping today",
                "10.0.0.5,1025,Buy wool sweaters!",
            };
 
            var userToIp = new[]
            {
                "u1,10.0.0.1",
                "u2,10.0.0.2",
                "u3,10.0.0.3",
                "u4,10.0.0.4",
                "u5,10.0.0.5",
            };

            // ip -> userId
            var ipToUser = userToIp
                .Select(line => line.Split(','))
                .ToDictionary(parts => parts[1], parts => parts[0], StringComparer.OrdinalIgnoreCase);

             // users who purchased at least once
            var buyers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var line in purchases)
                buyers.Add(line.Split(',')[0].Trim());

              // adText -> (purchases, clicks)
            var stats = new Dictionary<string, (int Purchases, int Clicks)>(StringComparer.OrdinalIgnoreCase);
            
            foreach (var line in clicks)
            {
                var parts = line.Split(',');
                var ip = parts[0].Trim();
                var adText = parts[2].Trim();

                if (!stats.ContainsKey(adText))
                    stats[adText] = (0, 0);

                stats[adText] = (stats[adText].Purchases, stats[adText].Clicks + 1);

                if (ipToUser.TryGetValue(ip, out var userId) && buyers.Contains(userId))
                {
                    stats[adText] = (stats[adText].Purchases + 1, stats[adText].Clicks);
                }
             }

             foreach (var (adText, stat) in stats)
                 Console.WriteLine($"\"{adText}\": {stat.Purchases} purchases / {stat.Clicks} clicks");

             Console.WriteLine("===========================================================================================");


             Console.WriteLine("7. Longest common browsing history** Two users' page-visit arrays. Find the **longest contiguous** sequence appearing in both.");
             
             var user1 = new[] { "/home", "/pricing", "/product", "/cart", "/checkout", "/confirm" };
             var user2 = new[] { "/pricing", "/product", "/cart", "/blog", "/checkout", "/confirm" };

            int maxLength = 0;
            int endIndexUser1 = 0;

            for (int i = 0; i < user1.Length; i++)
            {
                for (int j = 0; j < user2.Length; j++)
                {
                    int length = 0;
                    while (i + length < user1.Length && j + length < user2.Length && user1[i + length] == user2[j + length])
                    {
                        length++;
                    }

                    if (length > maxLength)
                    {
                        maxLength = length;
                        endIndexUser1 = i + length - 1;
                    }
                }
            }

            var longestSequence = user1.Skip(endIndexUser1 - maxLength + 1).Take(maxLength).ToArray();
            Console.WriteLine("Longest common browsing history: " + string.Join(", ", longestSequence));

            Console.WriteLine("===========================================================================================");

            Console.WriteLine("8. Most common page sequence (3-page path)** Given (user, page, timestamp) tuples, find the most common ordered 3-page sequence across users (contiguous per user).");

              var visits = new List<Visit>
            {
                new("u1", "/home",     1),
                new("u1", "/pricing",  2),
                new("u1", "/cart",     3),
                new("u1", "/checkout", 4),
 
                new("u2", "/home",     5),
                new("u2", "/pricing",  6),
                new("u2", "/cart",     7),
 
                new("u3", "/blog",     1),
                new("u3", "/home",     2),
                new("u3", "/pricing",  3),
                new("u3", "/cart",     4),
 
                new("u4", "/home",     9),
                new("u4", "/about",   10),
            };

            var sequenceCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var visitsByUser = visits.GroupBy(v => v.User).ToDictionary(g => g.Key, g => g.OrderBy(v => v.Timestamp).ToList());

            foreach (var userVisits in visitsByUser.Values)
            {
                for (int i = 0; i < userVisits.Count - 2; i++)
                {
                    var sequence = $"{userVisits[i].Page} -> {userVisits[i + 1].Page} -> {userVisits[i + 2].Page}";
                    if (sequenceCounts.ContainsKey(sequence))
                    {
                        sequenceCounts[sequence]++;
                    }
                    else
                    {
                        sequenceCounts[sequence] = 1;
                    }
                }
            }

            var mostCommonSequence = sequenceCounts.OrderByDescending(kvp => kvp.Value).First().Key;
            Console.WriteLine("Most common 3-page sequence: " + mostCommonSequence);

            Console.WriteLine("===========================================================================================");

            Console.WriteLine("9. Shared courses for every student pair** Input: (student, course) pairs. For **every pair of students**, output the courses both take.");

            var enrollments = new List<(string, string)>
            {
                ("Ada",     "CS101"),
                ("Ada",     "CS229"),
                ("Charlie", "CS101"),
                ("Charlie", "MATH51"),
                ("Bob",     "MATH51"),
                ("Bob",     "CS229"),
                ("Bob",     "CS101"),
                ("Dana",    "ART10"),
            };

            var coursesByStudent = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
            enrollments.ForEach(enrollment =>
            {
                if(coursesByStudent.ContainsKey(enrollment.Item1))
                {
                    coursesByStudent[enrollment.Item1].Add(enrollment.Item2);
                }
                else
                {
                    coursesByStudent[enrollment.Item1] = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { enrollment.Item2 };
                }
            });

            var students = coursesByStudent.Keys.ToList();
            for(int i = 0; i < students.Count; i++)
            {
                for(int j = i + 1; j < students.Count; j++)
                {
                    var student1 = students[i];
                    var student2 = students[j];
                    var sharedCourses = coursesByStudent[student1].Intersect(coursesByStudent[student2]);
                    Console.WriteLine($"{student1} & {student2}: {string.Join(", ", sharedCourses)}");
                }
            }

            Console.WriteLine("===========================================================================================");

             Console.WriteLine("10 - Midpoint course in a prerequisite chain** Given prerequisite pairs forming a single linear chain `[[\"Foundations\",\"Core\"],[\"Core\",\"Advanced\"]]`, find the course at the halfway point (round down for even length).");

              var pairs1 = new List<string[]>
            {
                new[] { "Foundations", "Core" },
                new[] { "Core", "Advanced" },
            };
            
            var next = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var hasPrereq = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
 
            foreach (var p in pairs1)
            {
                next[p[0]] = p[1];
                hasPrereq.Add(p[1]);
            }
 
            string start1 = next.Keys.First(c => !hasPrereq.Contains(c));
 
            var chain = new List<string> { start1 };
            while (next.TryGetValue(chain[^1], out var following))
                chain.Add(following);
 

            Console.WriteLine("Midpoint course: " + chain[(chain.Count - 1) / 2]);

            Console.WriteLine("===========================================================================================");

            Console.WriteLine("11. Find a word in a grid (straight lines only)**Does a word appear in a 2D char grid going only left→right or top→bottom (no bends)?");

            var grid = new[]
            {
                new[] { 'c', 'c', 'x', 't', 'i', 'b' },
                new[] { 'c', 'c', 'a', 't', 'n', 'i' },
                new[] { 'a', 'c', 'n', 'n', 't', 't' },
                new[] { 't', 'c', 's', 'i', 'p', 't' },
                new[] { 'a', 'o', 'o', 'o', 'a', 'a' },
                new[] { 'o', 'a', 'a', 'a', 'o', 'o' },
                new[] { 'k', 'a', 'i', 'c', 'k', 'i' },
            };

            string word = "cat";
            bool found = false;
            // Check rows
            for (int i = 0; i < grid.Length; i++)
            {
                var rowString = new string(grid[i]);
                if (rowString.Contains(word))
                {
                    found = true;
                    break;
                }
            }

            // Check columns
            if (!found)
            {
                for (int j = 0; j < grid[0].Length; j++)
                {
                    var columnString = new string(grid.Select(row => row[j]).ToArray());
                    if (columnString.Contains(word))
                    {
                        found = true;
                        break;
                    }
                }
            }

            Console.WriteLine($"Word \"{word}\" found in grid: {found}");

            Console.WriteLine("===========================================================================================");

            Console.WriteLine("12. Number of islands (connected 1s)** Count groups of connected 1s in a binary grid (4-directional).");

             var gridIslands = new[]
            {
                new[] { 1, 1, 0, 0, 0 },
                new[] { 1, 1, 0, 0, 0 },
                new[] { 0, 0, 1, 0, 0 },
                new[] { 0, 0, 0, 1, 1 },
            };

            int CountIslands(int[][] grid)
            {
                int count = 0;
                int rows = grid.Length;
                int cols = grid[0].Length;
                var visited = new bool[rows, cols];

                void DFS(int r, int c)
                {
                    if (r < 0 || r >= rows || c < 0 || c >= cols || grid[r][c] == 0 || visited[r, c])
                        return;

                    visited[r, c] = true;

                    DFS(r - 1, c); // up
                    DFS(r + 1, c); // down
                    DFS(r, c - 1); // left
                    DFS(r, c + 1); // right
                }

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        if (grid[i][j] == 1 && !visited[i, j])
                        {
                            count++;
                            DFS(i, j);
                        }
                    }
                }

                return count;
            }

            int islandCount = CountIslands(gridIslands);
            Console.WriteLine($"Number of islands: {islandCount}");

             Console.WriteLine("===========================================================================================");

             Console.WriteLine("*13. Rectangle of 0s in a grid of 1s** A grid of 1s contains exactly one rectangle of 0s. Return its top-left and bottom-right coordinates.");

             var gridRectangle = new[]
            {
                new[] { 1, 1, 1, 1, 1, 1 },
                new[] { 1, 1, 0, 0, 1, 1 },
                new[] { 1, 1, 0, 0, 1, 1 },
                new[] { 1, 1, 0, 0, 1, 1 },
                new[] { 1, 1, 1, 1, 1, 1 },
            };

            ((int Row, int Col) TopLeft, (int Row, int Col) BottomRight) FindRectangle(int[][] grid)
            {
                int rows = grid.Length;
                int cols = grid[0].Length;
                (int Row, int Col) topLeft = (-1, -1);
                (int Row, int Col) bottomRight = (-1, -1);

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        if (grid[i][j] == 0)
                        {
                            if (topLeft.Row == -1)
                            {
                                topLeft = (i, j);
                            }
                            bottomRight = (i, j);
                        }
                    }
                }

                return (topLeft, bottomRight);
            }   

            var (topLeft, bottomRight) = FindRectangle(gridRectangle);
            Console.WriteLine($"Top-left: ({topLeft.Row}, {topLeft.Col}), Bottom-right: ({bottomRight.Row}, {bottomRight.Col})");

             Console.WriteLine("===========================================================================================");

            Console.WriteLine("14. Matrix rotation / zero-out** Rotate an N×N matrix 90° in place, or: if a cell is 0, zero its entire row and column.");

             var a = new[]
            {
                new[] { 1, 2, 3 },
                new[] { 4, 5, 6 },
                new[] { 7, 8, 9 },
            };

            void Rotate90(int[][] matrix)
            {
                int n = matrix.Length;
                for (int i = 0; i < n / 2; i++)
                {
                    for (int j = i; j < n - i - 1; j++)
                    {
                        int temp = matrix[i][j];
                        matrix[i][j] = matrix[n - j - 1][i];
                        matrix[n - j - 1][i] = matrix[n - i - 1][n - j - 1];
                        matrix[n - i - 1][n - j - 1] = matrix[j][n - i - 1];
                        matrix[j][n - i - 1] = temp;
                    }
                }
            }   

            Rotate90(a);
            Console.WriteLine("Rotated matrix:");   
            foreach (var row in a)
                Console.WriteLine(string.Join(" ", row));

             Console.WriteLine("===========================================================================================");

             Console.WriteLine("15. Valid sentence checker** A sentence is valid if: starts with a capital, ends with `.?!`, single spaces, no consecutive separators, numbers don't start with 0 (varies). Return true/false given the rule list.");

              var tests = new (string Sentence, bool Expected)[]
            {
                ("The quick brown fox jumps over the lazy dog.", true),
                ("She has 7 cats, 2 dogs, and 1 parrot!",        true),
                ("Is this valid?",                               true),
                ("Exactly 0 problems here.",                     true),
 
                ("the sentence starts lowercase.",               false), // rule 1
                ("No terminator at the end",                     false), // rule 2
                ("Double  space inside.",                        false), // rule 3
                ("Consecutive,, separators.",                    false), // rule 4
                ("Ends oddly!.",                                 false), // rule 4
                ("Costs 050 dollars.",                           false), // rule 5
                ("Tabs\tare not allowed.",                       false), // rule 6
                ("",                                             false),
            };

            foreach (var (sentence, expected) in tests)
            {
                bool isValid = IsValidSentence(sentence);
                Console.WriteLine($"\"{sentence}\" -> {isValid} (expected: {expected})");
            }

            bool IsValidSentence(string s)
            {
                if (string.IsNullOrEmpty(s))
                    return false;

                if (!char.IsUpper(s[0]))
                    return false;

                if (!".!?".Contains(s[^1]))
                    return false;

                for (int i = 0; i < s.Length - 1; i++)
                {
                    if (s[i] == ' ' && s[i + 1] == ' ')
                        return false;
                    if (",.!?;".Contains(s[i]) && ",.!?;".Contains(s[i + 1]))
                        return false;
                }

                var words = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (var word in words)
                {
                    if (int.TryParse(word, out int num) && word.StartsWith("0"))
                        return false;
                }

                if (s.Contains('\t'))
                    return false;

                return true;
            }

             Console.WriteLine("===========================================================================================");

             Console.WriteLine("16. String/sentence reversal preserving punctuation positions** Reverse word order in a sentence, or reverse letters while keeping punctuation fixed in place.");

            var sentence1 = "Hello, world! This is a test.";

            string ReverseWords(string s)
            {
                var words = s.Split(' ');
                Array.Reverse(words);
                return string.Join(' ', words);
            }

            string ReverseLettersPreservingPunctuation(string s)
            {
                var chars = s.ToCharArray();
                int left = 0, right = chars.Length - 1;

                while (left < right)
                {
                    if (!char.IsLetter(chars[left]))
                    {
                        left++;
                    }
                    else if (!char.IsLetter(chars[right]))
                    {
                        right--;
                    }
                    else
                    {
                        var temp = chars[left];
                        chars[left] = chars[right];
                        chars[right] = temp;
                        left++;
                        right--;
                    }
                }

                return new string(chars);
            }

            Console.WriteLine("Original: " + sentence1);
            Console.WriteLine("Reversed words: " + ReverseWords(sentence1));
            Console.WriteLine("Reversed letters (preserving punctuation): " + ReverseLettersPreservingPunctuation(sentence1));

            Console.WriteLine("===========================================================================================");

            Console.WriteLine("17. Wrap text to width N (greedy word wrap)** Given words and a max line width, produce lines without breaking words.");

              var words = "The quick brown fox jumps over the lazy dog near the riverbank".Split(' ');

            List<string> WrapText(string[] words, int maxWidth)
            {
                var lines = new List<string>();
                var currentLine = new List<string>();
                int currentLength = 0;

                foreach (var word in words)
                {
                    if (currentLength + word.Length + currentLine.Count > maxWidth)
                    {
                        lines.Add(string.Join(' ', currentLine));
                        currentLine.Clear();
                        currentLength = 0;
                    }
                    currentLine.Add(word);
                    currentLength += word.Length;
                }

                if (currentLine.Count > 0)
                {
                    lines.Add(string.Join(' ', currentLine));
                }

                return lines;
            }   

            var wrappedLines = WrapText(words, 20);
            Console.WriteLine("Wrapped text:"); 

            foreach (var line in wrappedLines)
                Console.WriteLine(line);

             Console.WriteLine("===========================================================================================");

             Console.WriteLine("*18. Basic calculator / expression evaluation** Evaluate `\"2+3*4\"` (no parens), then with parentheses as the follow-up.");

             string expression = "2+3*4";

            int EvaluateExpression(string expr)
            {
                var tokens = new List<string>();
                int numberBuffer = 0;
                bool bufferingNumber = false;

                foreach (var ch in expr)
                {
                    if (char.IsDigit(ch))
                    {
                        numberBuffer = numberBuffer * 10 + (ch - '0');
                        bufferingNumber = true;
                    }
                    else
                    {
                        if (bufferingNumber)
                        {
                            tokens.Add(numberBuffer.ToString());
                            numberBuffer = 0;
                            bufferingNumber = false;
                        }
                        tokens.Add(ch.ToString());
                    }
                }

                if (bufferingNumber)
                {
                    tokens.Add(numberBuffer.ToString());
                }

                // First pass: handle multiplication
                var stack = new Stack<string>();
                for (int i = 0; i < tokens.Count; i++)
                {
                    if (tokens[i] == "*")
                    {
                        int left = int.Parse(stack.Pop());
                        int right = int.Parse(tokens[++i]);
                        stack.Push((left * right).ToString());
                    }
                    else
                    {
                        stack.Push(tokens[i]);
                    }
                }

                // Second pass: handle addition
                var resultStack = new Stack<string>(stack.Reverse());
                int result = int.Parse(resultStack.Pop());

                while (resultStack.Count > 0)
                {
                    string op = resultStack.Pop();
                    int nextNum = int.Parse(resultStack.Pop());

                    if (op == "+")
                    {
                        result += nextNum;
                    }
                }

                return result;
            }

            int resultExpr = EvaluateExpression(expression);
            Console.WriteLine($"Expression: {expression} = {resultExpr}");
            Console.WriteLine("===========================================================================================");    
            
            Console.WriteLine("19. Meeting calendar / free time slots** Given each person's busy intervals and working hours, find common free slots of at least K minutes for a meeting.");

              var busyPerPerson = new List<List<(string, string)>>
            {
                new() { ("0900", "1030"), ("1200", "1300"), ("1600", "1800") }, // person A
                new() { ("1000", "1130"), ("1230", "1430"), ("1530", "1630") }, // person B
            };
 
            var workingHours = new List<(string, string)>
            {
                ("0900", "2000"), // person A
                ("1000", "1830"), // person B
            };

            int K = 30; // meeting duration in minutes

            List<(string, string)> FindFreeSlots(List<List<(string, string)>> busyPerPerson, List<(string, string)> workingHours, int K)
            {
                var allBusy = new List<(DateTime Start, DateTime End)>();

                for (int i = 0; i < busyPerPerson.Count; i++)
                {
                    var personBusy = busyPerPerson[i];
                    var (workStartStr, workEndStr) = workingHours[i];
                    var workStart = DateTime.ParseExact(workStartStr, "HHmm", null);
                    var workEnd = DateTime.ParseExact(workEndStr, "HHmm", null);

                    allBusy.Add((workStart, workStart)); // start of working hours
                    allBusy.Add((workEnd, workEnd));     // end of working hours

                    foreach (var (startStr, endStr) in personBusy)
                    {
                        var start = DateTime.ParseExact(startStr, "HHmm", null);
                        var end = DateTime.ParseExact(endStr, "HHmm", null);
                        allBusy.Add((start, end));
                    }
                }

                allBusy = allBusy.OrderBy(b => b.Start).ThenBy(b => b.End).ToList();

                var mergedBusy = new List<(DateTime Start, DateTime End)>();
                foreach (var interval in allBusy)
                {
                    if (mergedBusy.Count == 0 || mergedBusy[^1].End < interval.Start)
                    {
                        mergedBusy.Add(interval);
                    }
                    else
                    {
                        mergedBusy[^1] = (mergedBusy[^1].Start, mergedBusy[^1].End > interval.End ? mergedBusy[^1].End : interval.End);
                    }
                }

                var freeSlots = new List<(string Start, string End)>();
                for (int i = 1; i < mergedBusy.Count; i++)
                {
                    var gapStart = mergedBusy[i - 1].End;
                    var gapEnd = mergedBusy[i].Start;
                    if ((gapEnd - gapStart).TotalMinutes >= K)
                    {
                        freeSlots.Add((gapStart.ToString("HHmm"), gapEnd.ToString("HHmm")));
                    }
                }

                return freeSlots;
            }

            var freeTimeSlots = FindFreeSlots(busyPerPerson, workingHours, K);
            Console.WriteLine("Available time slots for meeting:");
            foreach (var (start, end) in freeTimeSlots)
                Console.WriteLine($"- {start} to {end}");

             Console.WriteLine("===========================================================================================");

             Console.WriteLine("20. Sparse vector dot product** Two very large vectors that are mostly zeros. Design a representation and compute the dot product efficiently.");

              var a1 = new double[] { 0, 2, 0, 0, 3, 0, 0, 0, 1, 0 };
              var b2 = new double[] { 1, 4, 0, 0, 5, 0, 2, 0, 0, 0 };

            double SparseDotProduct(double[] a, double[] b)
            {
                var nonZeroA = a.Select((value, index) => (Value: value, Index: index)).Where(x => x.Value != 0).ToList();
                var nonZeroB = b.Select((value, index) => (Value: value, Index: index)).Where(x => x.Value != 0).ToDictionary(x => x.Index, x => x.Value);

                double dotProduct = 0;
                foreach (var (valueA, indexA) in nonZeroA)
                {
                    if (nonZeroB.TryGetValue(indexA, out var valueB))
                    {
                        dotProduct += valueA * valueB;
                    }
                }

                return dotProduct;
            }

            double dotProductResult = SparseDotProduct(a1, b2);
            Console.WriteLine($"Dot product of sparse vectors: {dotProductResult}");

           Console.WriteLine("===========================================================================================");

        }



    public record BadgeRecord(string Employee, string Room, string Action, string Time); // Action: "enter"/"exit", Time: "HHMM"
    public record Visit(string User, string Page, int Timestamp);


}}
