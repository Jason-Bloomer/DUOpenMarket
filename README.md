# DUOpenMarket
Open Source Third-Party Market API for Dual Universe

## What is the DUOpenMarket Desktop Client?
The DUOpenMarket Desktop client actively scrapes your logfiles while you play Dual Universe. It sends the market data for any items you search for ingame, to our servers, where we can share it with others who may not be logged into the game. We strongly encourage players to keep the DUOpenMarket Desktop client open in the background while they play the game normally. If everyone did that, our database would always be up to date.

## What information does DUOpenMarket collect?
The DUOpenMarket Desktop client ONLY accesses logfiles created by Dual Universe. Our servers occasionally collect IP information when a request is made, to help us improve the quality of our service and eliminate bots. We do not collect any personally identifiable information or other analytical data about our users, or their devices.

## Does the DUOpenMarket Client violate NovaQuark's TOS?
Simply put, NO. DUOpenMarket is perfectly legal to use and infact there are many tools that predate this one, which do similar things. The client only reads from log files which are created by Dual Universe. It does not read game files. It does not modify, update, move, or delete, any files other than its own. 



# DUOpenMarket API Developer Documentation

To use the API you will first need to log in through discord or obtain a valid Authorization code for a discord account, and supply it to the API as part of the URI string.

If the auth code is valid the server will accept commands from that user in the format:

` http://duopenmarket.xyz/openmarketapi.php/[DISCORD_AUTH_CODE]/[ACTION] `

Where [ACTION] is either create, read, update, or delete, followed by the appropriate URI variable inputs. Here is the Action string for an example API request:

` create?orderid=329482&marketid=53&itemid=125&quantity=467777&ordertype=1&expiration=12-12-2022%2055:55:55&lastupdated=12-12-2022%2055:55:55&price=66 `

The server's response will depend on the action:

    Create: A JSON-encapsulated True response, indicates the request succeeded. False indicates the request failed.
    Read: If request succeeds, the requested data is returned JSON-encapsulated. False indicates the request failed.
    Update: A JSON-encapsulated True response, indicates the request succeeded. False indicates the request failed.
    Delete: A JSON-encapsulated True response, indicates the request succeeded. False indicates the request failed.


## Planned improvements (In no particular order)
Sort items in the item dropdown, into their respective categories.

Format item order expiration date to be user-readable.

Add item statistics to the right side of window, below item name. Requires a neat way to look them up from a file. And the file. RegEx <3

Add the ability to keep track of the trailing decimal on order prices. (curently is rounded down)

Add a visual, interactive point-graph of the current orders for a given item.

Add Historical data retrieval commands to the API, and use this data to render visual graphs.

Add metastatistics about the overall health and throughput of the economy to the API, and graph them.

Add profit-margin and yeild-margin calculators for industry, which use the current market prices and can factor in talents.

Add fuel/warp-cost and transportation-related calculators using current fuel prices, factoring for talents.
