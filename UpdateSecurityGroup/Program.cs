using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Microsoft.Toolkit;

namespace UpdateSecurityGroup
{
    public class UpdateSecurityGroupProgram
    {
        //The website used to obtain our public IP address.
        public const string IPUrl = "http://ipinfo.io/ip";
        public const string Protocol = "tcp";

        private static async Task<DescribeSecurityGroupsResponse> GetSecurityGroupsFromPort(AmazonEC2Client ec2Client, int port)
        // Calls the AWS method to gather and return the Security Groups associated with the EC2 Client's VPC
        {
            return await ec2Client.DescribeSecurityGroupsAsync(new DescribeSecurityGroupsRequest
            {
                Filters = new List<Filter>
                {
                    new Filter
                    {
                        Name = "ip-permission.from-port",
                        Values = new List<string> {port.ToString()}
                    }
                }
            });
        }

        static async Task RemoveInboundRuleAsync(AmazonEC2Client ec2Client, string groupName, List<IpPermission> toDelete) 
        // Removes the permissions from the passed security group name listed in toDelete for the EC2 Client.
        {
            RevokeSecurityGroupIngressRequest request = new(groupName, toDelete);
            try
            {
                await ec2Client.RevokeSecurityGroupIngressAsync(request);
            }
            catch (Exception)
            {
                Console.WriteLine("Exception!");
            }
        }

        private static async Task AddInboundRuleAsync(AmazonEC2Client ec2Client, string groupID, string ipAddress, string protocol, int port)
        // Uses the passed parameters to build an "add" request object and then calls for AWS to add the rule.
        {
            
            AuthorizeSecurityGroupIngressResponse responseIngress;

            //Create a request object to hold the parameters for the rule to be added.
            AuthorizeSecurityGroupIngressRequest inboundRule = new()
            {
                GroupId = groupID
            };
            inboundRule.IpPermissions.Add(new IpPermission
            {
                IpProtocol = protocol,
                FromPort = port,
                ToPort = port,
                Ipv4Ranges = new List<IpRange>() { new IpRange { CidrIp = ipAddress } }
            });
            // Create the inbound rule for the security group
            try
            {
                responseIngress = await ec2Client.AuthorizeSecurityGroupIngressAsync(inboundRule);
            }
            catch (Exception)
            {
                Console.WriteLine("Exception in AddInboundRule");
            }
        }

       public static AWSCredentials GetAWSCredentials(string profile)
        // Obtains the AWS credentials
        {
            try
            {
                CredentialProfileStoreChain chain = new();
                if (chain.TryGetAWSCredentials(profile, out AWSCredentials awsCredentials))
                    return awsCredentials;
            } catch (Exception)
            {
                Console.WriteLine("Exception in GetAWSCredentials");
            }
            return null;
        }


        public static string GetExternalIP(string url)
        // Calls external web site to obtain current external IP address and validates address.
        {
            string externalip;
            try
            {
                externalip = new WebClient().DownloadString(url);
                if (externalip.IsIPAddress())
                {
                    return externalip;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Exception in GetExternalIP");
            }
            return string.Empty;
        }

        public static int GetPortNumber(string[] args)
        // Passed the args from the command line, it obtains the port number after performing validations.
        {
            if (args != null &&
                args.Length == 1 &&
                Int32.TryParse(args[0], out int port) &&
                port >= 1 && port <= 65535)
            {
                return port;
            }
            return 0;
        }

        static async Task<int> UpdateSecurityGroupsAsync(AmazonEC2Client ec2Client, DescribeSecurityGroupsResponse response, string ip, int port)
        // High level function that updates a security group's IP address for a given port.
        {
            int changed = 0;

            try
            {
                foreach (SecurityGroup group in response.SecurityGroups)
                {
                    List<IpPermission> toUpdate = new(group.IpPermissions.FindAll(i => i.FromPort == port).ToList<IpPermission>());
                    if (toUpdate.Count > 0)
                    {
                        await RemoveInboundRuleAsync(ec2Client, group.GroupName, group.IpPermissions);
                        await AddInboundRuleAsync(ec2Client, group.GroupId, ip, Protocol, port);
                        changed += 1;
                    }
                }
            } catch (Exception)
            {
                Console.WriteLine("Exception in UpdateSecurityGroups");
            }
            return changed;
        }

        static void PrintSecurityGroups(DescribeSecurityGroupsResponse groups, int port)
        // Displays the current list of security groups and their ingress rules.
        {
            try
            {
                ConsoleColor savedColor = Console.ForegroundColor;
                foreach (SecurityGroup group in groups.SecurityGroups)
                {
                    Console.WriteLine();
                    foreach (IpPermission ipPerm in group.IpPermissions)
                        foreach (IpRange range in ipPerm.Ipv4Ranges)
                        {
                            if (ipPerm.FromPort == port)
                                Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"{ group.GroupName } - { range.CidrIp }:{ ipPerm.FromPort.ToString() }");
                            Console.ForegroundColor = savedColor;
                        }
                }
                Console.WriteLine();
            } catch (Exception)
            {
                Console.WriteLine("Exception in PrintSecurityGroups");
            }
        }
        static async Task Main(string[] args)
        {
            //Set up
            int portNumber = GetPortNumber(args);
            string ip = GetExternalIP(IPUrl);
            AWSCredentials creds = GetAWSCredentials("secgroup");
            AmazonEC2Client ec2Client = new(creds);

            //If we have valid IP and port, as well as obtaining credentials, then update security groups
            if (portNumber > 0 && ip.Length > 0 && creds != null)
            {
                //AWS requires CIDR notation when specifying IP ranges. We are only updating a single IP, so append /32 to the IP address.
                ip += "/32";
                Console.WriteLine($"Updating port { portNumber.ToString() } for { ip.ToString() }");

                //Update the IPs for the specified port.
                DescribeSecurityGroupsResponse secGroups = await GetSecurityGroupsFromPort(ec2Client, portNumber);
                Console.WriteLine(await UpdateSecurityGroupsAsync(ec2Client, secGroups, ip, portNumber) + " IPs changed");
                
                //Get updated groups and output to screen.
                secGroups = await GetSecurityGroupsFromPort(ec2Client, portNumber);
                PrintSecurityGroups(secGroups, portNumber);

                //Keep the screen from disappeaing
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();

            }
        }
    }
}
