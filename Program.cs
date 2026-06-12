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
        }
    }


    public record BadgeRecord(string Employee, string Room, string Action, string Time); // Action: "enter"/"exit", Time: "HHMM"
    public record Visit(string User, string Page, int Timestamp);


}
