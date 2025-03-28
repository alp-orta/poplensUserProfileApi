--
-- PostgreSQL database dump
--

-- Dumped from database version 17.0
-- Dumped by pg_dump version 17.0

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: AspNetRoleClaims; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."AspNetRoleClaims" (
    "Id" integer NOT NULL,
    "RoleId" text NOT NULL,
    "ClaimType" text,
    "ClaimValue" text
);


ALTER TABLE public."AspNetRoleClaims" OWNER TO postgres;

--
-- Name: AspNetRoleClaims_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."AspNetRoleClaims" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."AspNetRoleClaims_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: AspNetRoles; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."AspNetRoles" (
    "Id" text NOT NULL,
    "Name" character varying(256),
    "NormalizedName" character varying(256),
    "ConcurrencyStamp" text
);


ALTER TABLE public."AspNetRoles" OWNER TO postgres;

--
-- Name: AspNetUserClaims; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."AspNetUserClaims" (
    "Id" integer NOT NULL,
    "UserId" text NOT NULL,
    "ClaimType" text,
    "ClaimValue" text
);


ALTER TABLE public."AspNetUserClaims" OWNER TO postgres;

--
-- Name: AspNetUserClaims_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."AspNetUserClaims" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."AspNetUserClaims_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: AspNetUserLogins; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."AspNetUserLogins" (
    "LoginProvider" text NOT NULL,
    "ProviderKey" text NOT NULL,
    "ProviderDisplayName" text,
    "UserId" text NOT NULL
);


ALTER TABLE public."AspNetUserLogins" OWNER TO postgres;

--
-- Name: AspNetUserRoles; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."AspNetUserRoles" (
    "UserId" text NOT NULL,
    "RoleId" text NOT NULL
);


ALTER TABLE public."AspNetUserRoles" OWNER TO postgres;

--
-- Name: AspNetUserTokens; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."AspNetUserTokens" (
    "UserId" text NOT NULL,
    "LoginProvider" text NOT NULL,
    "Name" text NOT NULL,
    "Value" text
);


ALTER TABLE public."AspNetUserTokens" OWNER TO postgres;

--
-- Name: AspNetUsers; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."AspNetUsers" (
    "Id" text NOT NULL,
    "UserName" character varying(256),
    "NormalizedUserName" character varying(256),
    "Email" character varying(256),
    "NormalizedEmail" character varying(256),
    "EmailConfirmed" boolean NOT NULL,
    "PasswordHash" text,
    "SecurityStamp" text,
    "ConcurrencyStamp" text,
    "PhoneNumber" text,
    "PhoneNumberConfirmed" boolean NOT NULL,
    "TwoFactorEnabled" boolean NOT NULL,
    "LockoutEnd" timestamp with time zone,
    "LockoutEnabled" boolean NOT NULL,
    "AccessFailedCount" integer NOT NULL
);


ALTER TABLE public."AspNetUsers" OWNER TO postgres;

--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE public."__EFMigrationsHistory" OWNER TO postgres;

--
-- Data for Name: AspNetRoleClaims; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."AspNetRoleClaims" ("Id", "RoleId", "ClaimType", "ClaimValue") FROM stdin;
\.


--
-- Data for Name: AspNetRoles; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."AspNetRoles" ("Id", "Name", "NormalizedName", "ConcurrencyStamp") FROM stdin;
\.


--
-- Data for Name: AspNetUserClaims; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."AspNetUserClaims" ("Id", "UserId", "ClaimType", "ClaimValue") FROM stdin;
\.


--
-- Data for Name: AspNetUserLogins; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."AspNetUserLogins" ("LoginProvider", "ProviderKey", "ProviderDisplayName", "UserId") FROM stdin;
\.


--
-- Data for Name: AspNetUserRoles; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."AspNetUserRoles" ("UserId", "RoleId") FROM stdin;
\.


--
-- Data for Name: AspNetUserTokens; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."AspNetUserTokens" ("UserId", "LoginProvider", "Name", "Value") FROM stdin;
\.


--
-- Data for Name: AspNetUsers; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."AspNetUsers" ("Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "PhoneNumber", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnd", "LockoutEnabled", "AccessFailedCount") FROM stdin;
2dc65d5c-5388-4a3f-a5f5-916979257bd2	alp	ALP	alp@alp.com	ALP@ALP.COM	f	AQAAAAIAAYagAAAAEKxgoaG1vO+c4nGE9Re2ybdMblrlSx5VWPBCOHHG+di2TlCWpTvwyY0coCjCkEujOg==	7FE2S446FV5JJUAXP4UWQYCDOZM3RF7B	aeff7d03-1e61-40dc-8ade-c3be2554473d	\N	f	f	\N	t	0
7361f50b-4c00-4389-818d-24388317bee9	string	STRING	string	STRING	f	AQAAAAIAAYagAAAAEFP3uYB10+Fwp7eJcv9nsQAKKScUrUho9IcqJ9A4oCVwfCHnbX0bAXbrTxvTk3jPag==	CCBMV6WMHE2TOPUYT4AYHEPOI5XOM3YR	a3f7a9cf-27f9-487a-987f-36e2b4cf14fc	\N	f	f	\N	t	0
74163551-9e75-4b13-89aa-952d0a312364	alpalpal	ALPALPAL	alpalpa@alp.com	ALPALPA@ALP.COM	f	AQAAAAIAAYagAAAAEGl0fQbeXjhIkYvDVCrgVu2lJLmRMPpGieyhWoFiTVYDnpxTlHZApnmrJ5ova+Owpw==	RXXQMCMMOPLYEA2VNYDXPXG4YTJEKECL	e9eaa1a0-8a4d-4065-bbc9-93c3558ec3f4	\N	f	f	\N	t	0
5f623cc9-08a8-4d07-9f51-ce4a450d0bb8	alpp	ALPP	alpp@gmail.com	ALPP@GMAIL.COM	f	AQAAAAIAAYagAAAAENEz9kGnLycI0Mo2bkzQYaOc9I04gertLRAqjI32uGhfuKa0l5kuvQUCClw6lezX9A==	OQKA2AN3VUVXWFYXFUIZLFPQUSQ52IDF	9ff871c2-f4f7-4f47-8754-288b4bf2b901	\N	f	f	\N	t	0
84818436-d374-4816-89e6-0016d76779c1	alpo	ALPO	alpp@alpp.com	ALPP@ALPP.COM	f	AQAAAAIAAYagAAAAEFpjs0KP0Uknw0mR8bFigX/NSLtnnbA+Oa9f0xwIyPbuMcFYH94pDYvlNkpOUDqv3Q==	2FDBRWTKPMMVJRNUBGJIYTUN7BH6JEYB	09dc491c-ea2e-4b2e-ae54-8dac76a462b9	\N	f	f	\N	t	0
6f5719f7-37d9-4f68-bd5f-31d18038af7f	1	1	1@1.com	1@1.COM	f	AQAAAAIAAYagAAAAEE2iRL5LrzuQBlmOWgZms6gbqhxLSCV01Cxkdy+UFenjX/l8DASrXk1H17dLUzgJ6Q==	XJOEIWFKDWXMWNVLQFHDTGNORZ4TGYTW	c5187480-49e5-4b8d-967c-964266a066e2	\N	f	f	\N	t	0
1ad41403-5863-41cb-ab95-0cc6e2159a37	11	11	11@1.com	11@1.COM	f	AQAAAAIAAYagAAAAEK8RqRrkIChFbNs+dlZ1dh3bYGWKL32sdUb0Fbfg7zgexzzCZyUPrasOq1wDVQFi4A==	75YUZW3GZ2UI6GH3YYH24FEK5IJVXWFJ	9bcd2d43-a748-4e20-a011-a090823bb359	\N	f	f	\N	t	0
60e754e0-4102-4efb-802c-d746bb88a891	2	2	2@2.c	2@2.C	f	AQAAAAIAAYagAAAAEKhHR6WtOit9XY4H1wzQJxNFvOS0oKicr9KekBwqlCCdl6hZYM6a/QxJdd+yFQhJGA==	QBANOJPLX7XTBEODSCBUIM55QJMXPGMD	e147d079-1c1d-4753-830e-d78568cd36ec	\N	f	f	\N	t	0
1abada28-fb20-47c7-a3fa-affa10bd7d21	123	123	123@123.com	123@123.COM	f	AQAAAAIAAYagAAAAEA4WS4XEbw43LPwVMg91uNQXLA5jLuwxg7KyAIpfZscQrHeFyXd3fIxgs/M0g7GryQ==	4B3O7ZJ5ZOAV7LTLTMTA355ULGSE6OUW	dd516f7c-c481-4322-8947-57ff5c9cf66c	\N	f	f	\N	t	0
ddff5b89-6772-4a77-a46f-12ec004ae54a	132	132	132@132.com	132@132.COM	f	AQAAAAIAAYagAAAAEEhLUiJutCBoUT0cR2gC7TqrisDyW+sPJxddshkzrCNfIrU0r+5QE5m+m2h1g0bTbg==	OFON2TO3NZJ3CQM3Z5CE2SQYY6ZRZCDE	98edc5f3-3b6b-40f2-a0fe-9c262b9b942b	\N	f	f	\N	t	0
eef97700-e31f-433f-915c-42e38e880dce	asd	ASD	asd@asd.com	ASD@ASD.COM	f	AQAAAAIAAYagAAAAEK3swPCTS20fdrMUXX9707qIWhhwR1qjUsJMorWnqMQnJtMe39hOaNzGyrcd94zPww==	3W75F45XQWOPPV2KUVKQDE376K7CWES5	d03dad77-4b1f-4f23-98ab-137561195e48	\N	f	f	\N	t	0
42ea3412-7fa0-4666-8e3f-15d0fbaf6a69	asdasd	ASDASD	asdasd@asd.asd	ASDASD@ASD.ASD	f	AQAAAAIAAYagAAAAEHc9pdM25uUaiGQ8Nft7GYp1VNGPyqG5XTHBREV9vOeDxtOb6IDnziPOf4ZmBO5eEw==	2Q2UFLM7XYR2HDDIEPY5FWRTT3TBHKNK	385158e1-9c77-42f3-a5e0-11759c619dc4	\N	f	f	\N	t	0
7c2f6af4-eff8-4e75-b3c4-6b4a6cfefb97	aaa	AAA	aaa@aaa.com	AAA@AAA.COM	f	AQAAAAIAAYagAAAAELaWcvzdJayysBm8AxS93Ujfu7jiBjB0WBhy+bJdnhTSqSZRwvAshqbzej7lV4MYFQ==	6CISMYIV5CRZWVQKJL4PTMQQFIT3SR7M	595020e4-ec28-4719-bc55-d95e8681a3a6	\N	f	f	\N	t	0
bac2ef43-4d2f-4d2a-bb21-df53616d397a	askfm	ASKFM	asdkm@as.com	ASDKM@AS.COM	f	AQAAAAIAAYagAAAAEB9Ly4tpP4up9CIKus4t5zB5j6L4it7v2Kw+T5YzfL2QikIZT9Uglr9LoKc+g7VoSA==	4FTMRKYVNCPQBTSZYDBSGDYS4CY3VBZL	2d6d7224-870d-4af0-80d2-0d8aa31a54ae	\N	f	f	\N	t	0
94f83e0c-da54-4093-829c-29afbbaa41ab	amsda	AMSDA	aklfmma@s.com	AKLFMMA@S.COM	f	AQAAAAIAAYagAAAAEL3ppagYrPie4ly9ddqZc6X5z0x4Xzm47VFFs8BScAo0W07AIMMnpiVEB3yKEAgDMQ==	H7RXZX2FV5TTVRPEYT55GZBC6OYNFQ76	d2578039-03a6-4462-8369-9cb1969c4cc2	\N	f	f	\N	t	0
49c1f83d-9f24-4131-9613-2fc4e211151a	strinasg	STRINASG	string@c.c	STRING@C.C	f	AQAAAAIAAYagAAAAEK7b7kYbUekykTcPqFC6sdm4WuCQpeDfYfAHQfZgTBLqaz7iwLs5Ni/3J4RtTaOpCw==	7DUAATUD2KQQADO6VJR36SYGY2CBAD3E	c710c7c4-0834-48d3-923a-aa115b9b0cb3	\N	f	f	\N	t	0
ac5dd96c-1e25-4738-ac0f-dca005462b77	john_doe	JOHN_DOE	a@a.csom	A@A.CSOM	f	AQAAAAIAAYagAAAAEKcYjSUTESGqntNWj4pueYZwRFoYXfbGOgQ1JM5VMXu6oYlUiCInR5zxx1Sajoih1w==	3CWEIGDH7JWUJVEUTGUQUDL6KCSPUKVN	05e6d4f3-ccba-4c93-ad48-1c8abd51e76b	\N	f	f	\N	t	0
bfee2fcf-76a1-448b-b7be-dcbfe1edf254	newuserrr	NEWUSERRR	newuser@a.a	NEWUSER@A.A	f	AQAAAAIAAYagAAAAEC4DYRuPvhnz7mFRKZfKkVkLCrln1/3jXWdLmw0CIS/Bvj5mjZniGGaAZ5ct2SCFqg==	DJVAWRANKYSIBEYI5DWMEL5Q6X2I6HFV	ffa13b2a-3e53-42e5-b291-fd752e6121f4	\N	f	f	\N	t	0
afea0481-cf8b-4842-86ff-8764d0f8f057	moyenmoyen	MOYENMOYEN	moyen@moyen.com	MOYEN@MOYEN.COM	f	AQAAAAIAAYagAAAAEOy25CY20eWkTSCeGe3qY+BNiIaKoFiGCUHer7j/nZ4N/qrVsGiD4MfAT9AAAIF6TQ==	LXJT43VXFTW7EKW5XDV245XHPPUDYS4I	668cfe5f-0233-4a20-8078-cf03029fd95a	\N	f	f	\N	t	0
790db5fe-76a5-4e72-968b-9fcb3278f2b1	alpmoyne	ALPMOYNE	aæ.com	AÆ.COM	f	AQAAAAIAAYagAAAAECC4l5nvN8QiRgG1JgrqEXIi0KKoOWNPOOaj+X2K5DMlZkHOlhn2Mr1L4lbVa+CBcg==	DXJAOGLVPTMND3VLD4KBKPRUE7OEBGTJ	73121bc8-e613-413f-9898-5845ded727e4	\N	f	f	\N	t	0
d7b42eda-4efb-4b7c-a4b7-20ea3001a6f3	lp	LP	lp@lp.c	LP@LP.C	f	AQAAAAIAAYagAAAAENy9GerOGhGMggzli03BUVu7CXfM/TIhHrx8xFN1JfXStc5+ckU+9cJH6ppsrB7R1w==	KXZ6M7BIQ3HM55KY35ACUBULXSYKAWK3	2cfb65df-3e3d-4c7e-bbf0-c7a3cd49ffaa	\N	f	f	\N	t	0
f27873ac-d62a-462a-b5e9-1c3e01e313b0	aadasd	AADASD	asdlaf@asd.cc	ASDLAF@ASD.CC	f	AQAAAAIAAYagAAAAEAGUFXyxyzDDfnyOYGvXfp6cB55fSnWlSyY+mIEd9eioOqhE+sIv2OYkjfDuo/Jt2A==	U5NJABQD2X2EDU6QWREJJIIZKEBD26Y4	d5bd1c20-9653-4463-b4c8-57d18955fbed	\N	f	f	\N	t	0
040f15c8-6a1e-4533-be52-e80e1e1fd65a	allp	ALLP	allp@allp.allp	ALLP@ALLP.ALLP	f	AQAAAAIAAYagAAAAEDnzbwce2zRtfvqgjFHoquQPBFpubnkYePFgQxH76//Qf5IRfuVQDEV9LST91ZsvzA==	LRZ3ICYISSCE3YE77EVC47ONTCMB6KEI	632197f7-b1fd-487e-ae71-152935531f69	\N	f	f	\N	t	0
f5b1f283-a915-4d6e-9c6d-aa0dd6f4a337	hkorcun	HKORCUN	feritkorcun@gmail.com	FERITKORCUN@GMAIL.COM	f	AQAAAAIAAYagAAAAECOuw34LuvMfA79TadkYikSCWuB8+HQ4AbZYARiRJcYzYnfYjWtRpasE9444sDWFcA==	YTZQRMFX3F24KPUCAGEZIZ3PBT6XHQ7B	dfa15552-4ac4-4cc2-9278-bd1bfe9a2673	\N	f	f	\N	t	0
3b5dad26-3097-41aa-9010-a0eb4aa56ed0	moyen	MOYEN	moyen@gmail.com	MOYEN@GMAIL.COM	f	AQAAAAIAAYagAAAAEGgtedPJiE6OyEAolJ3AbjILWZ1DlyWKH7eGwMweB6VXJBAC4pj8AquMmWRwlFQKgQ==	FQ7NBBSA7B5MHYGZVBLGCQB6FFNM35CR	c7c7514f-2eb7-4e1f-8639-63a355e1ccb4	\N	f	f	\N	t	0
dcf1a727-02f6-4d9b-bb11-3abe2ad97756	moyenalt	MOYENALT	moyenalt@gmail.com	MOYENALT@GMAIL.COM	f	AQAAAAIAAYagAAAAEOW7ULOsMZNgYZufJkt0fR/JuK4GSmsOq6JDl7IsdhDhSFUoZrKtF151aLavRwProQ==	X6CHNKQBMQ3WFW2BUGOSYLZ4V3K2A5UT	88ac3fa3-9e87-40a6-aa62-01dda9ae730a	\N	f	f	\N	t	0
7c81f94a-9175-45ac-85c7-29d50fb7e5fa	korcun	KORCUN	korcun@gmail.com	KORCUN@GMAIL.COM	f	AQAAAAIAAYagAAAAEK2V9djWsOTHhF5ggu54peOjYAme4w/PXqOvYPU8XKJDJFHGZBP6qwwQeGJiriRRSg==	W4SS7UUBXCSGHCI2P4YA6YEBNVOMEYP4	621d95ea-fa85-48e7-bff4-8cd11f2db165	\N	f	f	\N	t	0
\.


--
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
20241021194543_InitialCreate	8.0.10
\.


--
-- Name: AspNetRoleClaims_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."AspNetRoleClaims_Id_seq"', 1, false);


--
-- Name: AspNetUserClaims_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."AspNetUserClaims_Id_seq"', 1, false);


--
-- Name: AspNetRoleClaims PK_AspNetRoleClaims; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetRoleClaims"
    ADD CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY ("Id");


--
-- Name: AspNetRoles PK_AspNetRoles; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetRoles"
    ADD CONSTRAINT "PK_AspNetRoles" PRIMARY KEY ("Id");


--
-- Name: AspNetUserClaims PK_AspNetUserClaims; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetUserClaims"
    ADD CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY ("Id");


--
-- Name: AspNetUserLogins PK_AspNetUserLogins; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetUserLogins"
    ADD CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey");


--
-- Name: AspNetUserRoles PK_AspNetUserRoles; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetUserRoles"
    ADD CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId");


--
-- Name: AspNetUserTokens PK_AspNetUserTokens; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetUserTokens"
    ADD CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name");


--
-- Name: AspNetUsers PK_AspNetUsers; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetUsers"
    ADD CONSTRAINT "PK_AspNetUsers" PRIMARY KEY ("Id");


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- Name: EmailIndex; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "EmailIndex" ON public."AspNetUsers" USING btree ("NormalizedEmail");


--
-- Name: IX_AspNetRoleClaims_RoleId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON public."AspNetRoleClaims" USING btree ("RoleId");


--
-- Name: IX_AspNetUserClaims_UserId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AspNetUserClaims_UserId" ON public."AspNetUserClaims" USING btree ("UserId");


--
-- Name: IX_AspNetUserLogins_UserId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AspNetUserLogins_UserId" ON public."AspNetUserLogins" USING btree ("UserId");


--
-- Name: IX_AspNetUserRoles_RoleId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_AspNetUserRoles_RoleId" ON public."AspNetUserRoles" USING btree ("RoleId");


--
-- Name: RoleNameIndex; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX "RoleNameIndex" ON public."AspNetRoles" USING btree ("NormalizedName");


--
-- Name: UserNameIndex; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX "UserNameIndex" ON public."AspNetUsers" USING btree ("NormalizedUserName");


--
-- Name: AspNetRoleClaims FK_AspNetRoleClaims_AspNetRoles_RoleId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetRoleClaims"
    ADD CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES public."AspNetRoles"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserClaims FK_AspNetUserClaims_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetUserClaims"
    ADD CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserLogins FK_AspNetUserLogins_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetUserLogins"
    ADD CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserRoles FK_AspNetUserRoles_AspNetRoles_RoleId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetUserRoles"
    ADD CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES public."AspNetRoles"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserRoles FK_AspNetUserRoles_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetUserRoles"
    ADD CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserTokens FK_AspNetUserTokens_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."AspNetUserTokens"
    ADD CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- PostgreSQL database dump complete
--

