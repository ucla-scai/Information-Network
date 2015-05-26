DROP TABLE IF EXISTS Keyword;

DROP TABLE IF EXISTS Advertiser;

CREATE TABLE Keyword (
	Advertiser INT,
	Keyword INT,
	ShowPositions INT,
	Consumption INT,
	PRIMARY KEY (Advertiser, Keyword)
);

CREATE TABLE Advertiser (
	AdvertiserSector INT,
	ShowPositions INT,
	Impressions INT,
	Clicks INT,
	Consumption INT,
	BiddingPrice FLOAT,
	Quality FLOAT,
	CPC FLOAT,
	Rank FLOAT,
	Asn FLOAT
);


LOAD DATA LOCAL INFILE	'20130109_advertisers.dat' 
INTO TABLE 				Advertiser 
FIELDS TERMINATED BY 	'\t' 
OPTIONALLY ENCLOSED BY 	'"' 
ESCAPED BY 				'\\'
LINES TERMINATED BY 	'\n'
IGNORE 1 LINES;	

LOAD DATA LOCAL INFILE	'20130109_keywords.dat' 
INTO TABLE 				Keyword 
FIELDS TERMINATED BY 	'\t' 
OPTIONALLY ENCLOSED BY 	'"' 
ESCAPED BY 				'\\'
LINES TERMINATED BY 	'\n'
IGNORE 1 LINES;	