<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="PBIEmbedded._Default" %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">

        window.onload = function () {
            // Client side click to embed a selected report.
            var el = document.getElementById("bEmbedReportAction");
            if (el.addEventListener) {
                el.addEventListener("click", updateEmbedReport, false);
            } else {
                el.attachEvent('onclick', updateEmbedReport);
            }

        };

        // Update embed report
        function updateEmbedReport() {
            // Check if the embed url was selected
            var embedUrl = document.getElementById('tb_EmbedURL').value;

            // To load a report do the following:
            // 1: Set the url
            // 2: Add a onload handler to submit the auth token
            var iframe = document.getElementById('iFrameEmbedReport');
            iframe.src = embedUrl;
            iframe.onload = postActionLoadReport;
        }

        // Post the auth token to the iFrame. 
        function postActionLoadReport() {

            // Get the app token.
            accessToken = document.getElementById('MainContent_reportAppTokenTextbox').value;

            // Construct the push message structure
            var m = { action: "loadReport", accessToken: accessToken };
            message = JSON.stringify(m);

            // Push the message.
            iframe = document.getElementById('iFrameEmbedReport');
            iframe.contentWindow.postMessage(message, "*");
        }

    </script>

    <!--1. Get Reports -->
    <div> 
        <asp:Panel ID="PanelReports" runat="server" Visible="true">
            <p><b class="step">Step 1</b>: Get reports in your workspace.</p>
            <table>
            <tr>
                <td>1. Click <b>Get Reports</b> to get all reports in your workspace.</td>
            </tr>

            <tr>
                <td><asp:Button ID="getReportsButton" runat="server" OnClick="getReportsButton_Click" Text="Get Reports" /></td>
            </tr>
            
            <tr>
                <td>REST operation app token</td>
            </tr>

            <tr>
                <td><asp:TextBox ID="appTokenTextbox" runat="server" Width="1024px"></asp:TextBox></td>
            </tr>

            <tr><td>Reports</td></tr>

            <tr>
                <td>
                    <asp:TextBox ID="tb_reportsResult" runat="server" Height="200px" Width="1024px" TextMode="MultiLine" Wrap="False"></asp:TextBox>
                </td>
            </tr>
        </table>
        </asp:Panel>
    </div>

    <!--2. Get app token for Report -->
    <div> 
        <asp:Panel ID="ReportAppTokenPanel" runat="server" Visible="true">
            <p><b class="step">Step 2</b>: Get a report app token.</p>
            <table>
            <tr>
                <td>1. Enter a report id for a report from Step 1.</td>
            </tr>
            <tr>  
                <td><asp:TextBox ID="getReportAppTokenTextBox" runat="server" Width="1024px" Wrap="False"></asp:TextBox></td>
            </tr>
           <tr>
                <td>2. Click <b>Get Report app token</b> to get a report app token.</td>
            </tr>
            <tr>
                <td><asp:Button ID="getReportAppTokenButton" runat="server" OnClick="getReportAppTokenButton_Click" Text="Get Report app token" /></td>
            </tr>
            <tr>
                <td>Report app token</td>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="reportAppTokenTextbox" runat="server" Width="1024px"></asp:TextBox>
                </td>
            </tr>

        </table>
        </asp:Panel>
    </div>

    <!-- Embed Report-->
    <div> 
        <asp:Panel ID="PanelEmbed" runat="server" Visible="true">
            <p><b class="step">Step 3</b>: Embed a report.</p>
            <table>
                <tr>
                    <td>1. Enter an embed url for a report from Step 1 (starts with https://).</td>
                </tr>
                <tr>
                    <td>
                        <input type="text" id="tb_EmbedURL" style="width: 1024px;" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <br/><b>NOTE<br/></b>
                        <ul>
                            <li>To <b>Hide Filter Pane</b>, add &filterPaneEnabled=false to the report embed Url.</li>
                            <li>To <b>Filter a report</b>, add a <b>Filter Query</b> to the report Embed Url.<br/><br/>
                            <b>Filter Query Syntax</b>: &$filter={tableName/fieldName} eq '{fieldValue}'<br/><br/>
                            <b>Filter Query Example</b>: &$filter=Store/Chain%20eq%20'Lindseys'</li>
                        </ul>
                    </td>
                </tr>
                 <tr>
                    <td>
                        2. Click <b>Embed Report</b> to embed the report url.
                    </td>
                </tr>


                <tr>
                    <td>
                        <input type="button" id="bEmbedReportAction" value="Embed Report" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <iframe ID="iFrameEmbedReport" src="" height="768px" width="1024px" frameborder="1" seamless></iframe>
                    </td>
                </tr>
        </table>
        </asp:Panel>
    </div>
</asp:Content>
