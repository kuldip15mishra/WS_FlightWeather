<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
  <xsl:output method="html" version="1.0" encoding="UTF-8" indent="yes" doctype-public="-//W3C//DTD XHTML 1.0 Transitional//EN" doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"/>
  <xsl:template match="FlightWeatherResponse">
    <xsl:variable name="BookingID" select="./ItineraryQueue/BookingID"/>
    <xsl:variable name="Contactno" select="./ItineraryQueue/BookingDetails/MobileNo"/>
    <xsl:variable name="CheckinUrl">
      <xsl:call-template name="WebCheckinUrl">
        <xsl:with-param name="path" select="./ItineraryQueue/AirlineCode"/>
        <xsl:with-param name="fltno" select="./ItineraryQueue/FlightNo" />
      </xsl:call-template>
    </xsl:variable>

    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
        <title>Flight Weather Status</title>
        <style type="text/css">
          .ReadMsgBody{width: 100%; background-color: #ffffff;}
          .ExternalClass{width: 100%; background-color: #ffffff;}
          body{ width:100%; background-color:#ffffff; margin:0; padding:0; font-family: Arial, Helvetica, sans-serif; }
          table {border-collapse:collapse; }
          table.deviceWidth + p.MsoNormal,
          table.deviceWidth + div.MsoNormal
          {
          font-size:0 !important;
          }

          @media only screen and (max-width: 640px)  {
          body[yahoo] .deviceWidth {width:440px!important; padding:0; }
          body[yahoo] .deviceWidth_1 {width:410px!important; padding:0; }
          body[yahoo] .center {text-align: center!important;}
          body[yahoo] .section_heading{ font-size:16px!important;; }
          }

          @media only screen and (max-width: 479px) {
          body[yahoo] .deviceWidth {width:280px!important; padding:0; }
          body[yahoo] .deviceWidth_1 {width:250px!important; padding:0; }
          body[yahoo] .center {text-align: center!important;}
          body[yahoo] .section_heading{ font-size:15px !important;}
          body[yahoo] .section_heading{ font-size:13px!important;; }
          }

        </style>
      </head>

      <body yahoo="fix" style="font-family: Arial, Helvetica, sans-serif; margin:0; padding:0;-webkit-text-size-adjust:none;">
        <script type="application/ld+json">
          {
          "@context": "http://schema.org",
          "@type": "FlightReservation",
          "reservationNumber": "<xsl:value-of select="./ItineraryQueue/PNR"/>",
          "reservationStatus": "http://schema.org/Confirmed",
          "url": "<xsl:value-of select="concat('https://support.makemytrip.com/BookingDetails.aspx?hdnBookingID=',$BookingID,'&amp;hdnPhoneNumber=',$Contactno)"/>",
          "underName": {
          "@type": "Person",
          "name": "<xsl:value-of select="concat(./ItineraryQueue/BookingDetails/FirstName,./ItineraryQueue/BookingDetails/LastName)"/>",
          "email": "<xsl:value-of select="./ItineraryQueue/BookingDetails/Email"/>"
          },
          "bookingAgent": {
          "@type": "Organization",
          "name": "MakeMyTrip",
          "url": "https://support.makemytrip.com/"
          },
          "confirmReservationUrl": "<xsl:value-of select="concat('https://support.makemytrip.com/BookingDetails.aspx?hdnBookingID=',$BookingID,'&amp;hdnPhoneNumber=',$Contactno)"/>",
          "cancelReservationUrl": "<xsl:value-of select="concat('https://support.makemytrip.com/BookingDetails.aspx?hdnBookingID=',$BookingID,'&amp;hdnPhoneNumber=',$Contactno)"/>",
          "modifyReservationUrl": "<xsl:value-of select="concat('https://support.makemytrip.com/BookingDetails.aspx?hdnBookingID=',$BookingID,'&amp;hdnPhoneNumber=',$Contactno)"/>",
          "reservationFor": {
          "@type": "Flight",
          "flightNumber": "<xsl:value-of select="./ItineraryFlightStatus/FlightNumber"/>",
          "airline": {
          "@type": "Airline",
          "name": "<xsl:call-template name="flightscode">
            <xsl:with-param name="path" select="./ItineraryFlightStatus/CarrierCode"/>
            <xsl:with-param name="fltno" select="./ItineraryFlightStatus/FlightNumber" />
          </xsl:call-template>",
          "iataCode": "<xsl:value-of select="./ItineraryFlightStatus/CarrierCode"/>"
          },
          "operatedBy": {
          "@type": "Airline",
          "name": "<xsl:call-template name="flightscode">
            <xsl:with-param name="path" select="./ItineraryFlightStatus/CarrierCode"/>
            <xsl:with-param name="fltno" select="./ItineraryFlightStatus/FlightNumber" />
          </xsl:call-template>",
          "iataCode": "<xsl:value-of select="./ItineraryFlightStatus/CarrierCode"/>"
          },
          "departureAirport": {
          "@type": "Airport",
          "name": "<xsl:value-of select="./ItineraryFlightStatus/DepCity"/>",
          "iataCode": "<xsl:value-of select="./ItineraryFlightStatus/DepAirport"/>"
          },
          "departureTime": "<xsl:value-of select="./ItineraryFlightStatus/DepartureDate"/>",
          <xsl:if test="not(./ItineraryFlightStatus/DepTerminal='' or ./ItineraryFlightStatus/DepTerminal=' ' or count(./ItineraryFlightStatus/DepTerminal)=0)">
            "departureTerminal": "<xsl:value-of select="./ItineraryFlightStatus/DepTerminal"/>",
          </xsl:if>
          "arrivalAirport": {
          "@type": "Airport",
          "name": "<xsl:value-of select="./ItineraryFlightStatus/ArrCity"/>",
          "iataCode": "<xsl:value-of select="./ItineraryFlightStatus/ArrAirPort"/>"
          },
          <xsl:if test="not(./ItineraryFlightStatus/ArrTerminal='' or ./ItineraryFlightStatus/ArrTerminal=' ' or count(./ItineraryFlightStatus/ArrTerminal)=0)">
            "arrivalTerminal": "<xsl:value-of select="./ItineraryFlightStatus/ArrTerminal"/>",
          </xsl:if>
          "arrivalTime": "<xsl:value-of select="./ItineraryFlightStatus/ArrivalDate"/>"
          },
          "ticketDownloadUrl": "<xsl:value-of select="concat('https://support.makemytrip.com/BookingDetails.aspx?hdnBookingID=',$BookingID,'&amp;hdnPhoneNumber=',$Contactno)"/>",
          "ticketPrintUrl": "<xsl:value-of select="concat('https://support.makemytrip.com/BookingDetails.aspx?hdnBookingID=',$BookingID,'&amp;hdnPhoneNumber=',$Contactno)"/>"
          }
        </script>
        <!-- Wrapper -->
        <table width="100%" border="0" cellpadding="0" cellspacing="0" align="center">
          <tr>
            <td width="100%" valign="top" bgcolor="#ffffff" style="padding-top:20px;">

              <!-- Start Header-->
              <table width="620" border="0" cellpadding="0" cellspacing="0" align="center" class="deviceWidth">
                <tr>
                  <td width="100%" bgcolor="#ffffff" style="border:solid 1px #b0b0b0; border-bottom:0; background:#26459c;">

                    <!-- Logo -->
                    <table border="0" cellpadding="0" cellspacing="0" align="left" class="deviceWidth">
                      <tr>
                        <td style="padding:5px 10px; border-right:solid 1px #546aab;">
                          <a href="http://www.makemytrip.com">
                            <img src="http://flights.makemytrip.com/makemytrip/images/flightimg/mmt_logo.jpg" alt="" border="0" />
                          </a>
                        </td>
                        <td width="15"></td>
                        <td style="font:bold 20px Arial, Helvetica, sans-serif;color:#fff;">
                          Flight Status
                          <xsl:if test="./Forecast !='' and ./ItineraryFlightStatus/FlightStatus !='C'">
                            and Weather Update
                          </xsl:if>
                        </td>
                      </tr>
                    </table>
                    <!-- End Logo -->
                  </td>
                </tr>
                <!-- contents -->
                <tr>
                  <td>
                    <!-- 2 Column Images & Text Side by Side -->
                    <table width="100%" border="0" cellpadding="0" cellspacing="0" class="deviceWidth">
                      <tr>
                        <td style="background:#f5f5f5; font-size:12px; color:#2f2f2f; font-weight:normal; text-align:left; font-family:Arial, Helvetica, sans-serif; vertical-align:top; border:solid 1px #b0b0b0; border-bottom:solid 1px #959595; border-top:0;" align="center">
                          <table width="100%" align="left" cellpadding="0" cellspacing="0" border="0" class="deviceWidth">
                            <!-- Flight details -->
                            <tr>
                              <td width="620" valign="top" style="padding-top:15px;padding-bottom:10px;">
                                <table border="0" align="left" cellspacing="0" cellpadding="0" class="deviceWidth" style="width:100%;">
                                  <tbody>
                                    <tr>
                                      <td width="10">
                                        <xsl:text disable-output-escaping="yes"><![CDATA[&nbsp;]]></xsl:text>
                                      </td>
                                      <td style="border:1px solid #d4d4d4; border-bottom:0;">
                                        <table width="100%" align="center" cellspacing="0" cellpadding="0" style="background:#f5f5f5;">
                                          <tbody>
                                            <tr>
                                              <td style="padding:10px;">
                                                <table width="100%" align="left" cellspacing="0" cellpadding="0" style="background:#f5f5f5;">
                                                  <tbody>
                                                    <tr>
                                                      <td>
                                                        <!--left section-->
                                                        <table width="280" align="left" cellspacing="0" cellpadding="0" >
                                                          <tbody>
                                                            <tr>
                                                              <td style="line-height:1; padding-right:8px;" width="40">
                                                                <xsl:variable name="flighticon">
                                                                  <xsl:call-template name="FlightsIcon">
                                                                    <xsl:with-param name="path" select="./ItineraryQueue/AirlineCode"/>
                                                                    <xsl:with-param name="fltno" select="./ItineraryQueue/FlightNo"/>
                                                                  </xsl:call-template>
                                                                </xsl:variable>
                                                                <img src="{$flighticon}" alt="{./ItineraryFlightStatus/CarrierName}" title="{./ItineraryFlightStatus/CarrierName}" style="display:block;" />
                                                              </td>
                                                              <td style="border-left:solid 1px #dedede;" width="10">
                                                              </td>
                                                              <td width="60">
                                                                <span style="font:normal 14px Arial, Helvetica, sans-serif;color:#2f2f2f;padding-right:5px;white-space:nowrap;">
                                                                  <xsl:value-of select="./ItineraryFlightStatus/CarrierName"/>
                                                                </span>
                                                                <br/>
                                                                <span style="font:normal 12px Arial, Helvetica, sans-serif; color:#878787;">
                                                                  <xsl:value-of select="./ItineraryQueue/AirlineCode"/>-<xsl:value-of select="./ItineraryQueue/FlightNo"/>
                                                                </span>
                                                              </td>
                                                              <xsl:choose>
                                                                <xsl:when test="./ItineraryFlightStatus/FlightStatus = 'S' and ./ItineraryFlightStatus/DepartureGateDelayMin &lt; 15">
                                                                  <td bgcolor="#5a9344" style="font:bold 16px Arial, Helvetica, sans-serif; text-align:center; padding-top:8px; padding-bottom:8px; border-radius:2px; padding-left:10px; padding-right:10px; color:#ffffff;" width="80">
                                                                    <p style="color:#ffffff; text-decoration:none;margin:0;" href="">ON TIME</p>
                                                                  </td>
                                                                </xsl:when>
                                                                <xsl:when test="./ItineraryFlightStatus/FlightStatus = 'S'">
                                                                  <td style="font:bold 16px Arial, Helvetica, sans-serif; text-align:center; padding-top:8px; padding-bottom:8px; border-radius:2px; padding-left:10px; padding-right:10px; color:#ffffff;background-color:#8B0000;" width="80">
                                                                    <p style="color:White; text-decoration:none;margin:0;" href="">DELAYED</p>
                                                                  </td>
                                                                </xsl:when>
                                                                <xsl:when test="./ItineraryFlightStatus/FlightStatus = 'L'">
                                                                  <td bgcolor="#5a9344" style="font:bold 16px Arial, Helvetica, sans-serif; text-align:center; padding-top:8px; padding-bottom:8px; border-radius:2px; padding-left:10px; padding-right:10px; color:#ffffff;" width="80">
                                                                    <p style="color:#ffffff; text-decoration:none;margin:0;" href="">LANDED</p>
                                                                  </td>
                                                                </xsl:when>
                                                                <xsl:when test="./ItineraryFlightStatus/FlightStatus = 'A'">
                                                                  <td bgcolor="#FF3300" style="font:bold 16px Arial, Helvetica, sans-serif; text-align:center; padding-top:8px; padding-bottom:8px; border-radius:2px; padding-left:10px; padding-right:10px; color:#ffffff;" width="80">
                                                                    <p style="color:#ffffff; text-decoration:none;margin:0;" href="">ACTIVE</p>
                                                                  </td>
                                                                </xsl:when>
                                                                <xsl:when test="./ItineraryFlightStatus/FlightStatus = 'C'">
                                                                  <td bgcolor="#D00000" style="font:bold 16px Arial, Helvetica, sans-serif; text-align:center; padding-top:8px; padding-bottom:8px; border-radius:2px; padding-left:10px; padding-right:10px; color:#ffffff;" width="80">
                                                                    <p style="color:#ffffff; text-decoration:none;margin:0;" href="">Cancelled</p>
                                                                  </td>
                                                                </xsl:when>
                                                                <xsl:otherwise></xsl:otherwise>
                                                              </xsl:choose>
                                                              <td style="border-right:solid 1px #dedede;" width="15">
                                                              </td>
                                                            </tr>
                                                          </tbody>
                                                        </table>
                                                        <!--left section ends-->
                                                        <!--right section-->
                                                        <table width="280" align="left" cellspacing="0" cellpadding="0" >
                                                          <tbody>
                                                            <tr>
                                                              <td style="line-height:1; padding:0 8px 0 10px;" width="250">
                                                                <span style="font:normal 14px Arial, Helvetica, sans-serif;color:#2f2f2f;">
                                                                  BookingID :
                                                                </span>
                                                                <span style="font:bold 14px Arial, Helvetica, sans-serif;color:#2f2f2f;">
                                                                  <xsl:value-of select="./ItineraryQueue/BookingID"/>
                                                                </span>
                                                                <br/>
                                                                <span style="font:normal 14px Arial, Helvetica, sans-serif; color:#2f2f2f;padding-left:36px">
                                                                  PNR :
                                                                </span>
                                                                <span style="font:bold 14px Arial, Helvetica, sans-serif;color:#2f2f2f;">
                                                                  <xsl:value-of select="./ItineraryQueue/PNR"/>
                                                                </span>
                                                              </td>
                                                            </tr>
                                                          </tbody>
                                                        </table>
                                                        <!--right section ends-->
                                                      </td>
                                                    </tr>
                                                  </tbody>
                                                </table>
                                              </td>
                                            </tr>
                                          </tbody>
                                        </table>
                                      </td>
                                      <td width="10">
                                        <xsl:text disable-output-escaping="yes"><![CDATA[&nbsp;]]></xsl:text>
                                      </td>
                                    </tr>
                                    <tr>
                                      <td width="10"></td>
                                      <td style="border:1px solid #d4d4d4;">
                                        <table width="100%" cellspacing="0" cellpadding="0" style="background:#fff;">
                                          <tbody>
                                            <tr>
                                              <td style="padding:10px 10px 0;">
                                                <table width="100%" cellspacing="0" cellpadding="0" style="background:#fff;">
                                                  <tbody>
                                                    <tr>
                                                      <td style="padding-bottom:10px;">
                                                        <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                                          <tbody>
                                                            <tr>
                                                              <xsl:choose>
                                                                <xsl:when test="./ItineraryFlightStatus/FlightStatus = 'S' and ./ItineraryFlightStatus/DepartureGateDelayMin &lt; 15">
                                                                  <td style="font:bold 12px Arial, Helvetica, sans-serif; color:white; border-bottom:solid 1px #e5e5e5; padding:8px 0 8px 5px;background-color:#5a9344;">
                                                                    Your flight is on time and is scheduled as below:
                                                                  </td>
                                                                </xsl:when>
                                                                <xsl:when test="./ItineraryFlightStatus/FlightStatus = 'S'">
                                                                  <td style="font:bold 12px Arial, Helvetica, sans-serif; color:white; border-bottom:solid 1px #e5e5e5; padding:8px 0 8px 5px;background-color:#8B0000;">
                                                                    Your flight is delayed by <xsl:value-of select="./ItineraryFlightStatus/DepartureGateDelayMin"/> mins and expected schedule is as below:
                                                                  </td>
                                                                </xsl:when>
                                                                <xsl:when test="./ItineraryFlightStatus/FlightStatus = 'A'">
                                                                  <td style="font:bold 12px Arial, Helvetica, sans-serif; color:#2f2f2f; border-bottom:solid 1px #e5e5e5; padding:8px 0 8px 5px;background-color:red;">
                                                                    Your flight is Active and was scheduled is as below:
                                                                  </td>
                                                                </xsl:when>
                                                                <xsl:when test="./ItineraryFlightStatus/FlightStatus = 'C'">
                                                                  <td style="font:bold 12px Arial, Helvetica, sans-serif; color:#fff; border-bottom:solid 1px #e5e5e5; padding:8px 0 8px 5px;background-color:#D00000;">
                                                                    Your flight has been Cancelled. Please contact Airline for more details.
                                                                  </td>
                                                                </xsl:when>
                                                              </xsl:choose>
                                                            </tr>
                                                            <tr>
                                                              <td valign="top">
                                                                <!-- Col 1 -->
                                                                <table width="280" border="0" align="left" cellspacing="0" cellpadding="0" class="deviceWidth">
                                                                  <tbody>
                                                                    <tr>
                                                                      <td>
                                                                        <table width="280" align="center" cellspacing="0" cellpadding="0" style="background:#fff;">
                                                                          <tbody>
                                                                            <!-- top heading -->
                                                                            <tr>
                                                                              <td style="padding:8px 5px;">
                                                                                <table width="100%" align="center" cellspacing="0" cellpadding="0" style="background:#fff;">
                                                                                  <tbody>
                                                                                    <!--flight detail box -->
                                                                                    <tr>
                                                                                      <td>
                                                                                        <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                                                                          <tbody>
                                                                                            <tr>
                                                                                              <td valign="top" style="font-size:12px; font-family:Arial, Helvetica, sans-serif;color:#878787;">
                                                                                                DEPARTS
                                                                                              </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                              <td valign="top" style="font-size:26px; font-family:Arial, Helvetica, sans-serif;color:#2f2f2f;">
                                                                                                <xsl:value-of select="./ItineraryFlightStatus/DepartureAirportCode"/>
                                                                                              </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                              <td valign="top" style="font-size:14px; font-family:Arial, Helvetica, sans-serif; color:#2f2f2f;">
                                                                                                <xsl:value-of select="./ItineraryFlightStatus/DepCity"/>
                                                                                                <xsl:if test="./ItineraryFlightStatus/FlightStatus != 'C'">
                                                                                                  <b>
                                                                                                    <xsl:if test="string-length(./ItineraryFlightStatus/DepTerminal) &gt; 0">
                                                                                                      ,Terminal <xsl:value-of select="./ItineraryFlightStatus/DepTerminal"/>
                                                                                                    </xsl:if>
                                                                                                  </b>
                                                                                                </xsl:if>
                                                                                              </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                              <xsl:if test="string-length(./ItineraryFlightStatus/DepAirport) &gt; 0 and ./ItineraryFlightStatus/FlightStatus != 'C'">
                                                                                                <td valign="top" style="font-size:12px; font-family:Arial, Helvetica, sans-serif;color:#2f2f2f;">
                                                                                                  <xsl:value-of select="./ItineraryFlightStatus/DepAirport"/>
                                                                                                </td>
                                                                                              </xsl:if>
                                                                                            </tr>
                                                                                            <xsl:choose>
                                                                                              <xsl:when test="./ItineraryFlightStatus/FlightStatus != 'C'">
                                                                                                <tr>
                                                                                                  <td valign="top" style="font-size:14px; font-family:Arial, Helvetica, sans-serif; color:#2f2f2f;padding-top:8px;">Scheduled Departure:</td>
                                                                                                </tr>
                                                                                                <tr>
                                                                                                  <td valign="top" style="font-size:14px; font-family:Arial, Helvetica, sans-serif; color:#2f2f2f;">
                                                                                                    <span>
                                                                                                      <xsl:value-of select="msxsl:format-date(./ItineraryFlightStatus/DepartureDate,'ddd, dd MMM ')"/>
                                                                                                    </span>
                                                                                                    <span style="font-weight:bold">
                                                                                                      <xsl:value-of select="concat(msxsl:format-time(./ItineraryFlightStatus/DepartureDate,'HH:mm '),'hrs')"/>
                                                                                                    </span>
                                                                                                  </td>
                                                                                                </tr>
                                                                                              </xsl:when>
                                                                                            </xsl:choose>
                                                                                            <xsl:choose>
                                                                                              <xsl:when test="./ItineraryFlightStatus/FlightStatus = 'C'"></xsl:when>
                                                                                              <xsl:when test="./ItineraryFlightStatus/EstimatedGateDeparture != '0001-01-01T00:00:00' and ./ItineraryFlightStatus/EstimatedGateDeparture != ./ItineraryFlightStatus/ScheduledGateDeparture">
                                                                                                <tr>
                                                                                                  <td valign="top" style="font-size:14px; font-family:Arial, Helvetica, sans-serif; color:#2f2f2f;padding-top:8px;">Estimated Departure:</td>
                                                                                                </tr>
                                                                                                <tr>
                                                                                                  <td valign="top" style="font-size:14px; font-family:Arial, Helvetica, sans-serif; color:#2f2f2f;">
                                                                                                    <span>
                                                                                                      <xsl:value-of select="msxsl:format-date(./ItineraryFlightStatus/EstimatedGateDeparture,'ddd, dd MMM ')"/>
                                                                                                    </span>
                                                                                                    <span style="font-weight:bold">
                                                                                                      <xsl:value-of select="concat(msxsl:format-time(./ItineraryFlightStatus/EstimatedGateDeparture,'HH:mm '),'hrs')"/>
                                                                                                    </span>
                                                                                                  </td>
                                                                                                </tr>
                                                                                              </xsl:when>
                                                                                            </xsl:choose>
                                                                                            <!--<xsl:if test="./ItineraryFlightStatus/ActualGateDeparture != '0001-01-01T00:00:00'">
                                                                                              <tr>
                                                                                                <td valign="top" style="font-size:14px; font-family:Arial, Helvetica, sans-serif; color:#2f2f2f;padding-top:8px;">Acutal Departure:</td>
                                                                                              </tr>
                                                                                              <tr>
                                                                                                <td valign="top" style="font-size:14px; font-family:Arial, Helvetica, sans-serif; color:#2f2f2f;">
                                                                                                  <span>
                                                                                                    <xsl:value-of select="msxsl:format-date(./ItineraryFlightStatus/ActualGateDeparture,'ddd, dd MMM ')"/>
                                                                                                  </span>
                                                                                                  <span style="font-weight:bold">
                                                                                                    <xsl:value-of select="concat(msxsl:format-time(./ItineraryFlightStatus/ActualGateDeparture,'HH:mm '),'hrs')"/>
                                                                                                  </span>
                                                                                                </td>
                                                                                              </tr>
                                                                                            </xsl:if>-->
                                                                                          </tbody>
                                                                                        </table>
                                                                                      </td>
                                                                                    </tr>
                                                                                    <!-- /detail box -->
                                                                                  </tbody>
                                                                                </table>
                                                                              </td>
                                                                            </tr>
                                                                            <!-- /top heading -->
                                                                          </tbody>
                                                                        </table>
                                                                      </td>
                                                                    </tr>
                                                                  </tbody>
                                                                </table>
                                                                <!-- /Col 1 -->
                                                                <!-- Col 2 -->
                                                                <table width="280" border="0" align="left" cellspacing="0" cellpadding="0" class="deviceWidth">
                                                                  <tbody>
                                                                    <tr>
                                                                      <td>
                                                                        <table width="280" align="center" cellspacing="0" cellpadding="0" style="background:#fff;">
                                                                          <tbody>
                                                                            <!-- top heading -->
                                                                            <tr>
                                                                              <td style="padding:8px 5px;">
                                                                                <table width="100%" align="center" cellspacing="0" cellpadding="0" style="background:#fff;">
                                                                                  <tbody>
                                                                                    <!--flight detail box -->
                                                                                    <tr>
                                                                                      <td>
                                                                                        <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                                                                          <tbody>
                                                                                            <tr>
                                                                                              <td valign="top" style="font-size:12px; font-family:Arial, Helvetica, sans-serif;color:#878787;">
                                                                                                ARRIVES
                                                                                              </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                              <td valign="top" style="font-size:26px; font-family:Arial, Helvetica, sans-serif;color:#2f2f2f;">
                                                                                                <xsl:value-of select="./ItineraryFlightStatus/ArrivalAirportCode"/>
                                                                                              </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                              <td valign="top" style="font-size:14px; font-family:Arial, Helvetica, sans-serif; color:#2f2f2f;">
                                                                                                <xsl:value-of select="./ItineraryFlightStatus/ArrCity"/>
                                                                                                <xsl:if test="./ItineraryFlightStatus/FlightStatus != 'C'">
                                                                                                  <b>
                                                                                                    <xsl:if test="string-length(./ItineraryFlightStatus/ArrTerminal) &gt; 0">
                                                                                                      ,Terminal <xsl:value-of select="./ItineraryFlightStatus/ArrTerminal"/>
                                                                                                    </xsl:if>
                                                                                                  </b>
                                                                                                </xsl:if>
                                                                                              </td>
                                                                                            </tr>
                                                                                            <tr>
                                                                                              <xsl:if test="string-length(./ItineraryFlightStatus/ArrAirPort) &gt; 0 and ./ItineraryFlightStatus/FlightStatus != 'C'">
                                                                                                <td valign="top" style="font-size:12px; font-family:Arial, Helvetica, sans-serif;color:#2f2f2f;">
                                                                                                  <xsl:value-of select="./ItineraryFlightStatus/ArrAirPort"/>
                                                                                                </td>
                                                                                              </xsl:if>
                                                                                            </tr>
                                                                                            <xsl:choose>
                                                                                              <xsl:when test="./ItineraryFlightStatus/FlightStatus != 'C'">
                                                                                                <tr>
                                                                                                  <td valign="top" style="font-size:14px; font-family:Arial, Helvetica, sans-serif; color:#2f2f2f;padding-top:8px;">Scheduled Arrival:</td>
                                                                                                </tr>
                                                                                                <tr>
                                                                                                  <td valign="top" style="font-size:14px; font-family:Arial, Helvetica, sans-serif; color:#2f2f2f;">
                                                                                                    <span>
                                                                                                      <xsl:value-of select="msxsl:format-date(./ItineraryFlightStatus/ArrivalDate,'ddd, dd MMM ')"/>
                                                                                                    </span>
                                                                                                    <span style="font-weight:bold">
                                                                                                      <xsl:value-of select="concat(msxsl:format-time(./ItineraryFlightStatus/ArrivalDate,'HH:mm '),'hrs')"/>
                                                                                                    </span>
                                                                                                  </td>
                                                                                                </tr>
                                                                                              </xsl:when>
                                                                                            </xsl:choose>
                                                                                            <xsl:choose>
                                                                                              <xsl:when test="./ItineraryFlightStatus/FlightStatus = 'C'"></xsl:when>
                                                                                              <xsl:when test="./ItineraryFlightStatus/EstimatedGateArrival != '0001-01-01T00:00:00' and ./ItineraryFlightStatus/EstimatedGateArrival != ./ItineraryFlightStatus/ScheduledGateArrival">
                                                                                                <tr>
                                                                                                  <td valign="top" style="font-size:14px; font-family:Arial, Helvetica, sans-serif; color:#2f2f2f;padding-top:8px;">Estimated Arrival:</td>
                                                                                                </tr>
                                                                                                <tr>
                                                                                                  <td valign="top" style="font-size:14px; font-family:Arial, Helvetica, sans-serif; color:#2f2f2f;">
                                                                                                    <span>
                                                                                                      <xsl:value-of select="msxsl:format-date(./ItineraryFlightStatus/EstimatedGateArrival,'ddd, dd MMM ')"/>
                                                                                                    </span>
                                                                                                    <span style="font-weight:bold">
                                                                                                      <xsl:value-of select="concat(msxsl:format-time(./ItineraryFlightStatus/EstimatedGateArrival,'HH:mm '),'hrs')"/>
                                                                                                    </span>
                                                                                                  </td>
                                                                                                </tr>
                                                                                              </xsl:when>
                                                                                            </xsl:choose>
                                                                                            <!--<xsl:if test="./ItineraryFlightStatus/ActualGateArrival != '0001-01-01T00:00:00'">
                                                                                              <tr>
                                                                                                <td valign="top" style="font-size:14px; font-family:Arial, Helvetica, sans-serif; color:#2f2f2f;padding-top:8px;">Acutal Arrival:</td>
                                                                                              </tr>
                                                                                              <tr>
                                                                                                <td valign="top" style="font-size:14px; font-family:Arial, Helvetica, sans-serif; color:#2f2f2f;">
                                                                                                  <span>
                                                                                                    <xsl:value-of select="msxsl:format-date(./ItineraryFlightStatus/ActualGateArrival,'ddd, dd MMM ')"/>
                                                                                                  </span>
                                                                                                  <span style="font-weight:bold">
                                                                                                    <xsl:value-of select="concat(msxsl:format-time(./ItineraryFlightStatus/ActualGateArrival,'HH:mm '),'hrs')"/>
                                                                                                  </span>
                                                                                                </td>
                                                                                              </tr>
                                                                                            </xsl:if>-->
                                                                                          </tbody>
                                                                                        </table>
                                                                                      </td>
                                                                                    </tr>
                                                                                    <!-- /detail box -->
                                                                                  </tbody>
                                                                                </table>
                                                                              </td>
                                                                            </tr>
                                                                            <!-- /top heading -->
                                                                          </tbody>
                                                                        </table>
                                                                      </td>
                                                                    </tr>
                                                                  </tbody>
                                                                </table>
                                                                <!-- /Col 2 -->
                                                              </td>
                                                            </tr>
                                                          </tbody>
                                                        </table>
                                                      </td>
                                                    </tr>
                                                  </tbody>
                                                </table>
                                              </td>
                                            </tr>
                                          </tbody>
                                        </table>
                                      </td>
                                      <td width="10">
                                        <xsl:text disable-output-escaping="yes"><![CDATA[&nbsp;]]></xsl:text>
                                      </td>
                                    </tr>
                                  </tbody>
                                </table>
                                <!-- /Col 1 -->
                              </td>
                            </tr>
                            <!-- /Flight details -->
                            <xsl:if test="./Forecast !='' and ./ItineraryFlightStatus/FlightStatus !='C'">
                              <!-- weather heading -->
                              <tr>
                                <td valign="top" style="padding:10px; font:bold 14px Arial, Helvetica, sans-serif; color:#2f2f2f;text-transform:capitalize;">
                                  Weather Forecast - <span style="text-transform:uppercase;">
                                    <xsl:value-of select="./ItineraryFlightStatus/ArrCity"/>
                                  </span>
                                </td>
                              </tr>
                              <!-- /weather heading -->
                              <!-- weather details -->
                              <tr>
                                <td width="620" valign="top" style="padding-bottom:10px;">
                                  <table border="0" align="left" cellspacing="0" cellpadding="0" style="width:100%;" class="deviceWidth">
                                    <tbody>
                                      <tr>
                                        <td width="10">
                                          <xsl:text disable-output-escaping="yes"><![CDATA[&nbsp;]]></xsl:text>
                                        </td>
                                        <td style="border:1px solid #d4d4d4; background:#ffffff; padding-top:10px; padding-bottom:10px;">
                                          <table cellspacing="0" cellpadding="0" width="100%" border="0">
                                            <!-- Row -->
                                            <tr>
                                              <td style="border-bottom:solid 1px #d4d4d4; padding-bottom:10px;">
                                                <!-- left table -->
                                                <table width="280" align="left" cellspacing="0" cellpadding="0" style="background:#fff;">
                                                  <tbody>
                                                    <tr>
                                                      <td width="70" style="padding:0 8px; background:#ffffff;">
                                                        <table width="75" cellspacing="0" cellpadding="0" border="0">
                                                          <tbody>
                                                            <tr>
                                                              <td style="font:normal 14px Arial, Helvetica, sans-serif; color:#2f2f2f;padding-bottom:4px;text-align:center;">
                                                                <xsl:value-of select="./Forecast/simpleforecast/forecastday/Forecastday2[period='1']/date/weekday"/>
                                                              </td>
                                                            </tr>
                                                            <tr>
                                                              <td style="font:normal 12px Arial, Helvetica, sans-serif; color:#878787;text-align:center;">
                                                                <xsl:value-of select="./Forecast/simpleforecast/forecastday/Forecastday2[period='1']/date/day"/>
                                                                <xsl:value-of select="./Forecast/simpleforecast/forecastday/Forecastday2[period='1']/date/monthname_short"/>
                                                              </td>
                                                            </tr>
                                                          </tbody>
                                                        </table>
                                                      </td>
                                                      <td width="75" style="padding:0 8px; background:#ffffff; line-height:1;" valign="middle">
                                                        <xsl:variable name="imagePath">
                                                          <xsl:value-of select="./Forecast/simpleforecast/forecastday/Forecastday2[period='1']/icon_url"/>
                                                        </xsl:variable>
                                                        <img src="{$imagePath}" border="0" alt="" style="display:block;" align="center" />

                                                        <span style="text-align:center;font-size:12px;">
                                                          <xsl:value-of select="./Forecast/simpleforecast/forecastday/Forecastday2[period='1']/conditions"/>
                                                        </span>
                                                      </td>
                                                      <td width="45" style="padding:0 8px; background:#ffffff;">
                                                        <table width="50" cellspacing="0" cellpadding="0" border="0">
                                                          <tbody>
                                                            <tr>
                                                              <td style="font:normal 22px Arial, Helvetica, sans-serif; color:#2f2f2f;padding-bottom:2px;">
                                                                <xsl:value-of select="./Forecast/simpleforecast/forecastday/Forecastday2[period='1']/high/celsius"/>
                                                                <xsl:text disable-output-escaping="yes"><![CDATA[&deg;]]></xsl:text>
                                                                <sup style="font-size: 14px; vertical-align: top; line-height: 18px;">C</sup>
                                                              </td>
                                                            </tr>
                                                            <tr>
                                                              <td style="font:normal 12px Arial, Helvetica, sans-serif; color:#878787;">
                                                                MAX
                                                              </td>
                                                            </tr>
                                                          </tbody>
                                                        </table>
                                                      </td>
                                                      <td width="50" style="padding:0 8px 0 12px; background:#ffffff; border-left:solid 1px #e5e5e5;" >
                                                        <table width="45" cellspacing="0" cellpadding="0" border="0">
                                                          <tbody>
                                                            <tr>
                                                              <td style="font:normal 22px Arial, Helvetica, sans-serif; color:#2f2f2f;padding-bottom:2px;">
                                                                <xsl:value-of select="./Forecast/simpleforecast/forecastday/Forecastday2[period='1']/low/celsius"/>
                                                                <xsl:text disable-output-escaping="yes"><![CDATA[&deg;]]></xsl:text>
                                                                <sup style="font-size: 14px; vertical-align: top; line-height: 18px;">C</sup>
                                                              </td>
                                                            </tr>
                                                            <tr>
                                                              <td style="font:normal 12px Arial, Helvetica, sans-serif; color:#878787;">
                                                                MIN
                                                              </td>
                                                            </tr>
                                                          </tbody>
                                                        </table>
                                                      </td>
                                                      <!--<td style="padding:5px 8px 0; font:normal 12px Arial, Helvetica, sans-serif;color:#878787;" valign="middle">
                                                        <span style="display: inline-block; padding-bottom: 5px;">
                                                          <b>Morning: </b>
                                                          <xsl:value-of select="./Forecast/txt_forecast/forecastday/Forecastday[period='0']/fcttext_metric"/>
                                                        </span>
                                                        <span style="display: inline-block;">
                                                          <b>Evening: </b>
                                                          <xsl:value-of select="./Forecast/txt_forecast/forecastday/Forecastday[period='1']/fcttext_metric"/>
                                                        </span>
                                                      </td>-->
                                                    </tr>
                                                  </tbody>
                                                </table>
                                                <!-- /left table -->
                                                <!-- right table -->
                                                <table width="280" align="left" cellspacing="0" cellpadding="0" style="background:#fff;">
                                                  <tbody>
                                                    <tr>
                                                      <td style="padding:5px 8px 0; font:normal 11px Arial, Helvetica, sans-serif;color:#878787;" valign="middle">
                                                        <span style="display: inline-block; padding-bottom: 5px;">
                                                          <b>Morning: </b>
                                                          <xsl:value-of select="./Forecast/txt_forecast/forecastday/Forecastday[period='0']/fcttext_metric"/>
                                                        </span>
                                                        <span style="display: inline-block;">
                                                          <b>Evening: </b>
                                                          <xsl:value-of select="./Forecast/txt_forecast/forecastday/Forecastday[period='1']/fcttext_metric"/>
                                                        </span>
                                                      </td>
                                                    </tr>
                                                  </tbody>
                                                </table>
                                                <!-- /right table -->
                                              </td>
                                            </tr>
                                            <!-- /Row -->
                                            <!-- Row -->
                                            <tr>
                                              <td style="border-bottom:solid 1px #d4d4d4; padding-bottom:10px; padding-top:10px;">
                                                <!-- left table -->
                                                <table width="280" align="left" cellspacing="0" cellpadding="0" style="background:#fff;">
                                                  <tbody>
                                                    <tr>
                                                      <td width="70" style="padding:0 8px; background:#ffffff;">
                                                        <table width="75" cellspacing="0" cellpadding="0" border="0">
                                                          <tbody>
                                                            <tr>
                                                              <td style="font:normal 14px Arial, Helvetica, sans-serif; color:#2f2f2f;padding-bottom:4px;text-align:center;">
                                                                <xsl:value-of select="./Forecast/simpleforecast/forecastday/Forecastday2[period='2']/date/weekday"/>
                                                              </td>
                                                            </tr>
                                                            <tr>
                                                              <td style="font:normal 12px Arial, Helvetica, sans-serif; color:#878787;text-align:center;">
                                                                <xsl:value-of select="./Forecast/simpleforecast/forecastday/Forecastday2[period='2']/date/day"/>
                                                                <xsl:value-of select="./Forecast/simpleforecast/forecastday/Forecastday2[period='2']/date/monthname_short"/>
                                                              </td>
                                                            </tr>
                                                          </tbody>
                                                        </table>
                                                      </td>
                                                      <td width="75" style="padding:0 8px; background:#ffffff; line-height:1;" valign="middle">
                                                        <xsl:variable name="imagePath">
                                                          <xsl:value-of select="./Forecast/simpleforecast/forecastday/Forecastday2[period='2']/icon_url"/>
                                                        </xsl:variable>
                                                        <img src="{$imagePath}" border="0" alt="" style="display:block;" align="center" />
                                                        <span style="text-align:center;font-size:12px;">
                                                          <xsl:value-of select="./Forecast/simpleforecast/forecastday/Forecastday2[period='2']/conditions"/>
                                                        </span>
                                                      </td>
                                                      <td width="45" style="padding:0 8px; background:#ffffff;">
                                                        <table width="50" cellspacing="0" cellpadding="0" border="0">
                                                          <tbody>
                                                            <tr>
                                                              <td style="font:normal 22px Arial, Helvetica, sans-serif; color:#2f2f2f;padding-bottom:2px;">
                                                                <xsl:value-of select="./Forecast/simpleforecast/forecastday/Forecastday2[period='2']/high/celsius"/>
                                                                <xsl:text disable-output-escaping="yes"><![CDATA[&deg;]]></xsl:text>
                                                                <sup style="font-size: 14px; vertical-align: top; line-height: 18px;">C</sup>
                                                              </td>
                                                            </tr>
                                                            <tr>
                                                              <td style="font:normal 12px Arial, Helvetica, sans-serif; color:#878787;">
                                                                MAX
                                                              </td>
                                                            </tr>
                                                          </tbody>
                                                        </table>
                                                      </td>
                                                      <td width="50" style="padding:0 8px 0 12px; background:#ffffff; border-left:solid 1px #e5e5e5;" >
                                                        <table width="45" cellspacing="0" cellpadding="0" border="0">
                                                          <tbody>
                                                            <tr>
                                                              <td style="font:normal 22px Arial, Helvetica, sans-serif; color:#2f2f2f;padding-bottom:2px;">
                                                                <xsl:value-of select="./Forecast/simpleforecast/forecastday/Forecastday2[period='2']/low/celsius"/>
                                                                <xsl:text disable-output-escaping="yes"><![CDATA[&deg;]]></xsl:text>
                                                                <sup style="font-size: 14px; vertical-align: top; line-height: 18px;">C</sup>
                                                              </td>
                                                            </tr>
                                                            <tr>
                                                              <td style="font:normal 12px Arial, Helvetica, sans-serif; color:#878787;">
                                                                MIN
                                                              </td>
                                                            </tr>
                                                          </tbody>
                                                        </table>
                                                      </td>
                                                      <!--<td style="padding:5px 8px 0; font:normal 12px Arial, Helvetica, sans-serif;color:#878787;" valign="middle">
                                                        <span style="display: inline-block; padding-bottom: 5px;">
                                                          <b>Morning: </b>
                                                          <xsl:value-of select="./Forecast/txt_forecast/forecastday/Forecastday[period='2']/fcttext_metric"/>
                                                        </span>
                                                        <span style="display: inline-block;">
                                                          <b>Evening: </b>
                                                          <xsl:value-of select="./Forecast/txt_forecast/forecastday/Forecastday[period='3']/fcttext_metric"/>
                                                        </span>
                                                      </td>-->
                                                    </tr>
                                                  </tbody>
                                                </table>
                                                <!-- /left table -->
                                                <!-- right table -->
                                                <table width="280" align="left" cellspacing="0" cellpadding="0" style="background:#fff;">
                                                  <tbody>
                                                    <tr>
                                                      <td style="padding:5px 8px 0; font:normal 11px Arial, Helvetica, sans-serif;color:#878787;" valign="middle">
                                                        <span style="display: inline-block; padding-bottom: 5px;">
                                                          <b>Morning: </b>
                                                          <xsl:value-of select="./Forecast/txt_forecast/forecastday/Forecastday[period='2']/fcttext_metric"/>
                                                        </span>
                                                        <span style="display: inline-block;">
                                                          <b>Evening: </b>
                                                          <xsl:value-of select="./Forecast/txt_forecast/forecastday/Forecastday[period='3']/fcttext_metric"/>
                                                        </span>
                                                      </td>
                                                    </tr>
                                                  </tbody>
                                                </table>
                                                <!-- /right table -->
                                              </td>
                                            </tr>
                                            <!-- /Row -->
                                            <!-- Row -->
                                            <tr>
                                              <td style="padding-top:10px;">
                                                <!-- left table -->
                                                <table width="280" align="left" cellspacing="0" cellpadding="0" style="background:#fff;">
                                                  <tbody>
                                                    <tr>
                                                      <td width="70" style="padding:0 8px; background:#ffffff;">
                                                        <table width="75" cellspacing="0" cellpadding="0" border="0">
                                                          <tbody>
                                                            <tr>
                                                              <td style="font:normal 14px Arial, Helvetica, sans-serif; color:#2f2f2f;padding-bottom:4px;text-align:center;">
                                                                <xsl:value-of select="./Forecast/simpleforecast/forecastday/Forecastday2[period='3']/date/weekday"/>
                                                              </td>
                                                            </tr>
                                                            <tr>
                                                              <td style="font:normal 12px Arial, Helvetica, sans-serif; color:#878787;text-align:center;">
                                                                <xsl:value-of select="./Forecast/simpleforecast/forecastday/Forecastday2[period='3']/date/day"/>
                                                                <xsl:value-of select="./Forecast/simpleforecast/forecastday/Forecastday2[period='3']/date/monthname_short"/>
                                                              </td>
                                                            </tr>
                                                          </tbody>
                                                        </table>
                                                      </td>
                                                      <td width="75" style="padding:0 8px; background:#ffffff; line-height:1;" valign="middle">
                                                        <xsl:variable name="imagePath">
                                                          <xsl:value-of select="./Forecast/simpleforecast/forecastday/Forecastday2[period='3']/icon_url"/>
                                                        </xsl:variable>
                                                        <img src="{$imagePath}" border="0" alt="" style="display:block;" align="center" />
                                                        <span style="text-align:center;font-size:12px;">
                                                          <xsl:value-of select="./Forecast/simpleforecast/forecastday/Forecastday2[period='3']/conditions"/>
                                                        </span>
                                                      </td>
                                                      <td width="45" style="padding:0 8px; background:#ffffff;">
                                                        <table width="50" cellspacing="0" cellpadding="0" border="0">
                                                          <tbody>
                                                            <tr>
                                                              <td style="font:normal 22px Arial, Helvetica, sans-serif; color:#2f2f2f;padding-bottom:2px;">
                                                                <xsl:value-of select="./Forecast/simpleforecast/forecastday/Forecastday2[period='3']/high/celsius"/>
                                                                <xsl:text disable-output-escaping="yes"><![CDATA[&deg;]]></xsl:text>
                                                                <sup style="font-size: 14px; vertical-align: top; line-height: 18px;">C</sup>
                                                              </td>
                                                            </tr>
                                                            <tr>
                                                              <td style="font:normal 12px Arial, Helvetica, sans-serif; color:#878787;">
                                                                MAX
                                                              </td>
                                                            </tr>
                                                          </tbody>
                                                        </table>
                                                      </td>
                                                      <td width="50" style="padding:0 8px 0 12px; background:#ffffff; border-left:solid 1px #e5e5e5;" >
                                                        <table width="45" cellspacing="0" cellpadding="0" border="0">
                                                          <tbody>
                                                            <tr>
                                                              <td style="font:normal 22px Arial, Helvetica, sans-serif; color:#2f2f2f;padding-bottom:2px;">
                                                                <xsl:value-of select="./Forecast/simpleforecast/forecastday/Forecastday2[period='3']/low/celsius"/>
                                                                <xsl:text disable-output-escaping="yes"><![CDATA[&deg;]]></xsl:text>
                                                                <sup style="font-size: 14px; vertical-align: top; line-height: 18px;">C</sup>
                                                              </td>
                                                            </tr>
                                                            <tr>
                                                              <td style="font:normal 12px Arial, Helvetica, sans-serif; color:#878787;">
                                                                MIN
                                                              </td>
                                                            </tr>
                                                          </tbody>
                                                        </table>
                                                      </td>
                                                      <!--<td style="padding:5px 8px 0; font:normal 12px Arial, Helvetica, sans-serif;color:#878787;" valign="middle">
                                                        <span style="display: inline-block; padding-bottom: 5px;">
                                                          <b>Morning: </b>
                                                          <xsl:value-of select="./Forecast/txt_forecast/forecastday/Forecastday[period='4']/fcttext_metric"/>
                                                        </span>
                                                        <span style="display: inline-block;">
                                                          <b>Evening: </b>
                                                          <xsl:value-of select="./Forecast/txt_forecast/forecastday/Forecastday[period='5']/fcttext_metric"/>
                                                        </span>
                                                      </td>-->
                                                    </tr>
                                                  </tbody>
                                                </table>
                                                <!-- /left table -->
                                                <!-- right table -->
                                                <table width="280" align="left" cellspacing="0" cellpadding="0" style="background:#fff;">
                                                  <tbody>
                                                    <tr>
                                                      <td style="padding:5px 8px 0; font:normal 11px Arial, Helvetica, sans-serif;color:#878787;" valign="middle">
                                                        <span style="display: inline-block; padding-bottom: 5px;">
                                                          <b>Morning: </b>
                                                          <xsl:value-of select="./Forecast/txt_forecast/forecastday/Forecastday[period='4']/fcttext_metric"/>
                                                        </span>
                                                        <span style="display: inline-block;">
                                                          <b>Evening: </b>
                                                          <xsl:value-of select="./Forecast/txt_forecast/forecastday/Forecastday[period='5']/fcttext_metric"/>
                                                        </span>
                                                      </td>
                                                    </tr>
                                                  </tbody>
                                                </table>
                                                <!-- /right table -->
                                              </td>
                                            </tr>
                                            <!-- /Row -->

                                          </table>
                                        </td>
                                        <td width="10">
                                          <xsl:text disable-output-escaping="yes"><![CDATA[&nbsp;]]></xsl:text>
                                        </td>
                                      </tr>
                                    </tbody>
                                  </table>
                                </td>
                              </tr>
                              <!-- /weather details -->
                            </xsl:if>
                            <!-- thanks message -->
                            <tr>
                              <td style="font:bold 14px Arial, Helvetica, sans-serif; padding:12px 10px 15px;">
                                Thanks <br/>
                                Team MakeMyTrip
                              </td>
                            </tr>
                            <!-- /thanks message -->
                          </table>
                          <!-- End 2 Column Images & Text Side by SIde -->
                        </td>
                      </tr>
                      <!-- /contents -->
                    </table>
                  </td>
                </tr>
              </table>
              <!-- End Contents -->
              <!-- Bottom outer contents -->
              <table width="100%" cellpadding="0" cellspacing="0" style="border:0;">
                <tr>
                  <td style="padding-top:10px;">
                    <table width="620" align="center" cellpadding="0" cellspacing="0" bgcolor="#ffffff" class="deviceWidth" style="border:0;">

                      <!-- Bottom inner contents -->
                      <tr>
                        <td width="620">
                          <table width="100%" cellpadding="0" cellspacing="0" style="border:0;text-align:center;background:#ffffff;">
                            <tr>

                              <td width="620" style="font-family:Arial, Helvetica, sans-serif; font-size:11px; color:#2f2f2f; padding:10px; vertical-align:middle; text-align:center; border-bottom:solid 1px #b0b0b0; line-height:15px;">
                                Disclaimer: Note that the above data is as per the latest information available with MakeMyTrip. Please contact airline for further enquiry. Ignore this email if you have cancelled your booking few hours before departure.
                              </td>
                            </tr>
                          </table>
                        </td>
                      </tr>
                      <!-- /Bottom inner contents -->

                      <tr>

                        <td style="font-family:Arial, Helvetica, sans-serif; font-size:11px; padding:10px; color:#878787; text-align:center; vertical-align:middle;">
                          Please do not reply to this mail. This is an autogenerated email. For any assistance or query please <br/> mark all your responses to  <a href="mailto:support@makemytrip.com">support@makemytrip.com</a>.
                        </td>

                      </tr>
                    </table>
                  </td>

                </tr>
              </table>
              <!-- /Bottom outer contents -->
            </td>
          </tr>
        </table>
        <!-- End Wrapper -->
      </body>
    </html>
  </xsl:template>
  <xsl:template name="flightscode">
    <xsl:param name="path"/>
    <xsl:param name="fltno"/>
    <xsl:choose>
      <xsl:when test="$path='SG'">Spicejet</xsl:when>
      <xsl:when test="$path='G8'">Go Air</xsl:when>
      <xsl:when test="$path='9H'">MDLR</xsl:when>
      <xsl:when test="$path='6E'">Indigo</xsl:when>
      <xsl:when test="$path='I7'">Paramount</xsl:when>
      <xsl:when test="$path='IT' and $fltno &lt; '3000'">Kingfisher Class</xsl:when>
      <xsl:when test="$path='IT' and $fltno > '2999'">Kingfisher Red</xsl:when>
      <xsl:when test="$path='9W'">
        <xsl:choose>
          <xsl:when test="$fltno &lt; '3000' and $fltno > '1999'">Jet Konnect</xsl:when>
          <xsl:when test="$fltno &lt; '8000' and $fltno > '6999'">Jet Lite Codeshare</xsl:when>
          <xsl:otherwise>Jet Airways</xsl:otherwise>
        </xsl:choose>
      </xsl:when>
      <xsl:when test="$path='IC'">Air India IC</xsl:when>
      <xsl:when test="$path='S2'">Jet Lite</xsl:when>
      <xsl:when test="$path='AI'">Air India</xsl:when>
      <xsl:when test="$path='IX'">Air India Express</xsl:when>
      <xsl:when test="$path='MH'">Malaysia</xsl:when>
      <xsl:when test="$path='AK'">Air Asia</xsl:when>
      <xsl:when test="$path='DC'">Deccan Shuttles</xsl:when>
    </xsl:choose>
  </xsl:template>
  <xsl:template name="FlightsIcon">
    <xsl:param name="path"/>
    <xsl:param name="fltno"/>
    <xsl:choose>
      <xsl:when test="$path='SG'">http://flights.makemytrip.com/makemytrip/images/flightimg/spicejet.gif</xsl:when>
      <xsl:when test="$path='G8'">http://www.makemytrip.com/images/goair.gif</xsl:when>
      <xsl:when test="$path='9H'">http://flights.makemytrip.com/makemytrip/images/flightimg/mdlr.gif</xsl:when>
      <xsl:when test="$path='6E'">http://flights.makemytrip.com/makemytrip/images/flightimg/indigo.gif</xsl:when>
      <xsl:when test="$path='I7'">http://flights.makemytrip.com/makemytrip/images/flightimg/paramount.GIF</xsl:when>
      <xsl:when test="$path='IT' and FlightNo &lt; '3000'">http://flights.makemytrip.com/makemytrip/images/flightimg/kingfisher.gif</xsl:when>
      <xsl:when test="$path='IT' and FlightNo > '2999'">http://flights.makemytrip.com/makemytrip/images/flightimg/airdeccan.gif</xsl:when>
      <xsl:when test="$path='9W'">
        <xsl:choose>
          <xsl:when test="$fltno &lt; '3000' and $fltno &gt; '1999'">http://flights.makemytrip.com/makemytrip/images/flightimg/jetKonnect.gif</xsl:when>
          <xsl:when test="$fltno &lt; '8000' and $fltno &gt; '6999'">http://flights.makemytrip.com/makemytrip/images/flightimg/jetLiteSpecial.gif</xsl:when>
          <xsl:otherwise>http://flights.makemytrip.com/makemytrip/images/flightimg/jetairways.gif</xsl:otherwise>
        </xsl:choose>
      </xsl:when>
      <xsl:when test="$path='IC'">http://flights.makemytrip.com/makemytrip/images/flightimg/indian_airlines.gif</xsl:when>
      <xsl:when test="$path='S2'">http://flights.makemytrip.com/makemytrip/images/flightimg/sahara.gif</xsl:when>
      <xsl:when test="$path='AI'">http://flights.makemytrip.com/makemytrip/images/flightimg/airIndia.gif</xsl:when>
      <xsl:when test="$path='IX'">http://cheapfaresindia.makemytrip.com/international/img/international/airline-logos/smAIE.gif</xsl:when>
      <xsl:when test="$path='DC'">http://flights.makemytrip.com/makemytrip/images/flightimg/deccan.gif</xsl:when>
      <xsl:when test="$path='LB'">http://flights.makemytrip.com/makemytrip/images/flightimg/airCosta.png</xsl:when>
    </xsl:choose>
  </xsl:template>
  <xsl:template name="WebCheckinUrl">
    <xsl:param name="path"/>
    <xsl:param name="fltno"/>
    <xsl:choose>
      <xsl:when test="$path='SG'">https://book.spicejet.com/SearchWebCheckin.aspx</xsl:when>
      <xsl:when test="$path='G8'">https://www.goair.in/WebCheckin/Web-CheckIn.aspx‎</xsl:when>
      <xsl:when test="$path='6E'">https://book.goindigo.in/Checkin‎</xsl:when>
      <xsl:when test="$path='IT'">http://www.flykingfisher.com/plan-book/web-check-in.aspx</xsl:when>
      <xsl:when test="$path='9W'">
        <xsl:choose>
          <xsl:when test="$fltno &lt; '3000' and $fltno > '1999'">www.jetkonnect.com/in/webcheckin.aspx‎</xsl:when>
          <xsl:otherwise>www.jetairways.com/EN/IN/PlanYourTravel/WebCheck-in.aspx‎</xsl:otherwise>
        </xsl:choose>
      </xsl:when>
      <xsl:when test="$path='IC'">https://fastcheck.sita.aero/cce-presentation-web-ai/entryUpdate.do</xsl:when>
      <xsl:when test="$path='S2'">http://www.jetairways.com/EN/IN/PlanYourTravel/WebCheck-in.aspx</xsl:when>
      <xsl:when test="$path='AI'">https://fastcheck.sita.aero/cce-presentation-web-ai/entryUpdate.do</xsl:when>
      <xsl:when test="$path='IX'">https://fastcheck.sita.aero/cce-presentation-web-ai/entryUpdate.do</xsl:when>
      <xsl:when test="$path='MH'">https://fastcheck.sita.aero/cce-presentation-web-mh/entryUpdate.do</xsl:when>
      <xsl:when test="$path='AK'">http://checkin.airasia.com/</xsl:when>
    </xsl:choose>
  </xsl:template>
</xsl:stylesheet>
