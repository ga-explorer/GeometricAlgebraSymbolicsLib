%Always begin with this
gapotInit;

clc;

%Create vector using MATLAB array
u = 5;
mvU = gapotTermsArrayToVector([100*sqrt(2)/u; 0]);

%Create bivector using text expressions
mvM = gapotParseBivector("2 <>, -5 <1,2>, 5<3,4>");

fprintf("Display the terms of GAPoT multivectors\n");
gapotDisplayTerms(mvU)
gapotDisplayTerms(mvM)

%Compute the geometric product of GAPoT multivectors
mvI = gapotGp(gapotInverse(mvU), mvM);

fprintf("Display the geometric product of GAPoT multivectors\n");
gapotDisplayTerms(mvI)