# DUOpenMarket
Open Source Third-Party Market API for Dual Universe

## What is the DUOpenMarket Desktop Client?
The DUOpenMarket Desktop client actively scrapes your logfiles while you play Dual Universe. It sends the market data for any items you search for ingame, to our servers, where we can share it with others who may not be logged into the game. We strongly encourage players to keep the DUOpenMarket Desktop client open in the background while they play the game normally. If everyone did that, our database would always be up to date.

## What information does DUOpenMarket collect?
The DUOpenMarket Desktop client ONLY accesses logfiles created by Dual Universe. Our servers occasionally collect IP information when a request is made, to help us improve the quality of our service and eliminate bots. We do not collect any personally identifiable information or other analytical data about our users, or their devices.

## Does the DUOpenMarket Client violate NovaQuark's TOS?
Simply put, NO. DUOpenMarket is perfectly legal to use and infact there are many tools that predate this one, which do similar things. The client only reads from log files which are created by Dual Universe. It does not read game files. It does not modify, update, move, or delete, any files other than its own. 



# DUOpenMarket API Developer Documentation




## Planned features/improvements (In no particular order)

:heavy_check_mark: Format item order expiration date to be user-readable.

:heavy_check_mark: Add the ability to keep track of the trailing decimal on order prices. (curently is rounded down)

:heavy_check_mark: Sort items in the item dropdown, into their respective categories.

:heavy_check_mark: Process orders in batches. (improve server read efficiency)

:heavy_check_mark: Custom sorting functions for order columns. (String values sort alphabetically, Number values sort numerically)

Add a "Resource Manager" panel or window, which can automatically update/restore/backup user scripts, holograms, and sounds.

Add item statistics to the right side of window, below item name. Requires a neat way to look them up from a file. And the file. RegEx <3

Add a visual, interactive point-graph of the current orders for a given item.

Add Historical data retrieval commands to the API, and use this data to render visual graphs.

Add metastatistics about the overall health and throughput of the economy to the API, and graph them.

Add profit-margin and yeild-margin calculators for industry, which use the current market prices and can factor in talents.

Add fuel/warp-cost and transportation-related calculators using current fuel prices, factoring for talents.

Add an interface for tracking relationships bewteen player accounts and discord ID's, for the purpose of providing a "reputation" for a given player.
