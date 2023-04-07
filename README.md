# Picrew <u>Gr</u>ou<u>p</u> Photo Generator
![version](https://img.shields.io/github/v/release/dninemfive/grp?color=brightgreen&label=Version) 
[![license](https://img.shields.io/badge/License-All%20rights%20reserved-blue.svg)](https://github.com/dninemfive/grp/blob/master/LICENSE)
[![githubdls](https://img.shields.io/github/downloads/dninemfive/grp/total?color=blue&label=Github&logo=github)](https://github.com/dninemfive/grp/releases/latest)

Program which generates a "group photo" of Discord users from images generated using [this picrew](https://picrew.me/en/image_maker/701767) and submitted via a Google Form (not included).
If properly configured, the program will automatically download the latest data from a Google Sheet attached to the Form.

Input has the following assumptions, validated by the Google Form:
- Discord discriminator matches `.+#\d{4}` (i.e. the username#1234 pattern);
- Submitted URL matches `https:\/\/.+\/.+\..+`, i.e. it is HTTPS and has a file name with an extension;
- and height matches `(\d+['‘’]\d+["“”]|\d+cm)`, i.e. it is in one of the following formats, where # is a number:
	- `#cm`, for height in centimeters;
	- `#'#"`, where the apostrophes and quotes can be either normal or "fancy" quotes, for height in feet and inches.
	
The code performs additional checks to discourage adversarial inputs but these are by no means complete and the form used should not necessarily be made public.