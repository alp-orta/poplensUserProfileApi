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

--
-- Name: uuid-ossp; Type: EXTENSION; Schema: -; Owner: -
--

CREATE EXTENSION IF NOT EXISTS "uuid-ossp" WITH SCHEMA public;


--
-- Name: EXTENSION "uuid-ossp"; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION "uuid-ossp" IS 'generate universally unique identifiers (UUIDs)';


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: Follows; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Follows" (
    "Id" uuid NOT NULL,
    "FollowerId" uuid NOT NULL,
    "FollowingId" uuid NOT NULL,
    "CreatedDate" timestamp with time zone DEFAULT '-infinity'::timestamp with time zone NOT NULL,
    "LastUpdatedDate" timestamp with time zone DEFAULT '-infinity'::timestamp with time zone NOT NULL
);


ALTER TABLE public."Follows" OWNER TO postgres;

--
-- Name: Profiles; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Profiles" (
    "Id" uuid NOT NULL,
    "UserId" text NOT NULL
);


ALTER TABLE public."Profiles" OWNER TO postgres;

--
-- Name: Reviews; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Reviews" (
    "Id" uuid NOT NULL,
    "Content" text NOT NULL,
    "Rating" real NOT NULL,
    "ProfileId" uuid NOT NULL,
    "MediaId" text NOT NULL,
    "CreatedDate" timestamp with time zone DEFAULT '-infinity'::timestamp with time zone NOT NULL,
    "LastUpdatedDate" timestamp with time zone DEFAULT '-infinity'::timestamp with time zone NOT NULL
);


ALTER TABLE public."Reviews" OWNER TO postgres;

--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE public."__EFMigrationsHistory" OWNER TO postgres;

--
-- Data for Name: Follows; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Follows" ("Id", "FollowerId", "FollowingId", "CreatedDate", "LastUpdatedDate") FROM stdin;
2ec4e024-4b3b-44ea-acd2-7c2f786bc092	1afc12b7-f15b-42ca-8e5f-ac1ba62cf0d3	afca5e96-07d0-4918-8ee4-ce851fcb4e69	2024-12-12 16:30:29.973674+03	2024-12-12 16:30:29.973729+03
76c0a903-f53a-4a0b-8858-b1f7408e1ef2	afca5e96-07d0-4918-8ee4-ce851fcb4e69	1afc12b7-f15b-42ca-8e5f-ac1ba62cf0d3	2024-12-12 16:30:56.877499+03	2024-12-12 16:30:56.877499+03
46a142c0-5d2b-47e5-9240-0aafa4a57a23	3b413a26-81cf-4a21-97bb-aa06f94680d4	003f8b2e-e60b-4c36-9cee-074eb1594088	2025-03-17 20:20:16.068766+03	2025-03-17 20:20:16.068766+03
fe14bdea-b60a-456e-8126-a780f3268803	3b413a26-81cf-4a21-97bb-aa06f94680d4	cc5b234d-a8eb-4b18-84e3-e0d7c19bbc59	2025-03-17 20:29:58.437713+03	2025-03-17 20:29:58.437713+03
652d33f7-4ec6-4b99-80dd-571254c902c5	003f8b2e-e60b-4c36-9cee-074eb1594088	cc5b234d-a8eb-4b18-84e3-e0d7c19bbc59	2025-03-17 20:48:13.898883+03	2025-03-17 20:48:13.898883+03
970a7b13-00eb-4861-9dbe-526fe85ab486	003f8b2e-e60b-4c36-9cee-074eb1594088	3b413a26-81cf-4a21-97bb-aa06f94680d4	2025-03-19 22:47:23.658364+03	2025-03-19 22:47:23.658488+03
\.


--
-- Data for Name: Profiles; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Profiles" ("Id", "UserId") FROM stdin;
1afc12b7-f15b-42ca-8e5f-ac1ba62cf0d3	2dc65d5c-5388-4a3f-a5f5-916979257bd2
afca5e96-07d0-4918-8ee4-ce851fcb4e69	7361f50b-4c00-4389-818d-24388317bee9
f059fd4b-0661-49e7-b8a1-9161d70370cf	74163551-9e75-4b13-89aa-952d0a312364
ea2d1225-9603-4175-9cdb-47b2f18225df	5f623cc9-08a8-4d07-9f51-ce4a450d0bb8
dcba7032-17ca-4703-86dc-95e1b18a3980	84818436-d374-4816-89e6-0016d76779c1
57854220-325b-4240-b9db-ec28c6980497	6f5719f7-37d9-4f68-bd5f-31d18038af7f
a6a690be-dbc9-424d-bd48-3b96ba8af4b7	1ad41403-5863-41cb-ab95-0cc6e2159a37
99709c0d-5db4-4788-b301-7522324d9675	60e754e0-4102-4efb-802c-d746bb88a891
caa98487-7629-41a1-9763-a3f95192f239	1abada28-fb20-47c7-a3fa-affa10bd7d21
9781f77e-3193-4e71-9628-b126346c4f39	ddff5b89-6772-4a77-a46f-12ec004ae54a
f4ffe790-22ec-46bf-8c7c-ea6a8d522a55	eef97700-e31f-433f-915c-42e38e880dce
20182bd5-9660-462a-bd76-f17ea921a77e	42ea3412-7fa0-4666-8e3f-15d0fbaf6a69
abdc5f63-cd85-4481-98cc-618910ff5f37	7c2f6af4-eff8-4e75-b3c4-6b4a6cfefb97
07b61f51-9c1a-41b4-8579-1437e5ea5221	bac2ef43-4d2f-4d2a-bb21-df53616d397a
13ba3342-4410-430e-985f-14c274f8f902	94f83e0c-da54-4093-829c-29afbbaa41ab
1d27d0fd-af86-4477-bc54-02e25f4a4d10	49c1f83d-9f24-4131-9613-2fc4e211151a
eaa3615f-f0c3-4d10-a95e-619370d45675	ac5dd96c-1e25-4738-ac0f-dca005462b77
f9ed69b1-0623-462c-8fa0-8fea5eaa096a	bfee2fcf-76a1-448b-b7be-dcbfe1edf254
2c5b70aa-fcbf-4774-9bb4-8a5321a6782e	afea0481-cf8b-4842-86ff-8764d0f8f057
9d318ef1-8d25-4851-8644-e466f9276863	790db5fe-76a5-4e72-968b-9fcb3278f2b1
073abada-8bf3-40ce-a942-3101ef348b75	d7b42eda-4efb-4b7c-a4b7-20ea3001a6f3
43709f7c-107b-48c7-98e1-b4f8c42f351e	f27873ac-d62a-462a-b5e9-1c3e01e313b0
1068b6b5-1cfb-4bdb-87ad-adbc2e53aae6	f5b1f283-a915-4d6e-9c6d-aa0dd6f4a337
003f8b2e-e60b-4c36-9cee-074eb1594088	3b5dad26-3097-41aa-9010-a0eb4aa56ed0
cc5b234d-a8eb-4b18-84e3-e0d7c19bbc59	dcf1a727-02f6-4d9b-bb11-3abe2ad97756
3b413a26-81cf-4a21-97bb-aa06f94680d4	7c81f94a-9175-45ac-85c7-29d50fb7e5fa
\.


--
-- Data for Name: Reviews; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Reviews" ("Id", "Content", "Rating", "ProfileId", "MediaId", "CreatedDate", "LastUpdatedDate") FROM stdin;
c31c6a87-7312-4948-b4f2-3dd78f4fd6a5	first review ever i dont know which film this is!	5	1afc12b7-f15b-42ca-8e5f-ac1ba62cf0d3	0c790b81-9cee-4134-aae0-355b8f69b8be	2024-12-12 16:07:37.115405+03	2024-12-12 16:07:37.115468+03
166ba745-9031-4034-92d8-fd5e78daf31d	Ne kadar güzel bir film!	5	003f8b2e-e60b-4c36-9cee-074eb1594088	149771ec-fbd3-4991-be3a-7700d7ff5b8a	2025-03-03 12:20:46.638279+03	2025-03-03 12:20:46.638387+03
6c0e4761-2e10-4d82-a3d9-892c4af3f3c8	good ahh movie	5	003f8b2e-e60b-4c36-9cee-074eb1594088	890fda3a-fd3b-4936-9837-f5a0d8210723	2025-03-08 17:38:06.666158+03	2025-03-08 17:38:06.666247+03
e5e93e6f-fc82-4dbb-a0bc-26cb3333c74e	idk man i dont read	5	003f8b2e-e60b-4c36-9cee-074eb1594088	ff8c8344-505d-4461-a40d-e5531b3d5ff4	2025-03-08 18:16:24.522294+03	2025-03-08 18:16:24.522542+03
1aaad880-7679-462f-b228-0136651bfc6e	so many memories	5	003f8b2e-e60b-4c36-9cee-074eb1594088	e0a8ddd6-378b-4975-ac28-c823392a5383	2025-03-09 22:27:13.200397+03	2025-03-09 22:27:13.201059+03
291027e1-a186-4dd6-97df-fce650c10472	this move was at least as looooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooong as this review	4	003f8b2e-e60b-4c36-9cee-074eb1594088	ba2f1e21-a227-458d-bc2d-8e99b12150a7	2025-03-09 22:43:56.210948+03	2025-03-09 22:43:56.210948+03
c3be3002-b926-48dc-915a-d3ccff445d34	this sucks	1	cc5b234d-a8eb-4b18-84e3-e0d7c19bbc59	0c023ad8-066b-4ded-b376-53d3c65ce711	2025-03-17 19:25:04.99136+03	2025-03-17 19:25:04.991657+03
a783d34c-34e1-4195-88e6-b96d770db212	nice movie for children magic does not exist	4	003f8b2e-e60b-4c36-9cee-074eb1594088	3d264721-5a13-4e11-a18e-39b555b4e927	2025-03-17 20:18:11.423098+03	2025-03-17 20:18:11.423179+03
ab97a3b5-86bd-4c44-b7d6-5298cbae58bd	i love sex	5	3b413a26-81cf-4a21-97bb-aa06f94680d4	23dffc7a-6524-4660-883b-4f7fd1a30209	2025-03-17 20:20:01.187851+03	2025-03-17 20:20:01.187852+03
78775ead-6e8f-4624-a857-4fadba5f2735	sex but darker	5	3b413a26-81cf-4a21-97bb-aa06f94680d4	9674040a-63e4-4f4b-8b84-d65518c9469a	2025-03-17 20:23:30.81338+03	2025-03-17 20:23:30.81338+03
\.


--
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
20241112155018_InitialCreate.	8.0.10
20241112161740_UpdateProfileReviewRelationship	8.0.10
20241112162607_UpdateReviewToNotHaveProfileNavigationProperty	8.0.10
20241212125421_AddTimestampsToFollowAndReview	8.0.10
\.


--
-- Name: Follows PK_Follows; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Follows"
    ADD CONSTRAINT "PK_Follows" PRIMARY KEY ("Id");


--
-- Name: Profiles PK_Profiles; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Profiles"
    ADD CONSTRAINT "PK_Profiles" PRIMARY KEY ("Id");


--
-- Name: Reviews PK_Reviews; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Reviews"
    ADD CONSTRAINT "PK_Reviews" PRIMARY KEY ("Id");


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- Name: IX_Follows_FollowerId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Follows_FollowerId" ON public."Follows" USING btree ("FollowerId");


--
-- Name: IX_Follows_FollowingId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Follows_FollowingId" ON public."Follows" USING btree ("FollowingId");


--
-- Name: IX_Reviews_ProfileId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Reviews_ProfileId" ON public."Reviews" USING btree ("ProfileId");


--
-- Name: Follows FK_Follows_Profiles_FollowerId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Follows"
    ADD CONSTRAINT "FK_Follows_Profiles_FollowerId" FOREIGN KEY ("FollowerId") REFERENCES public."Profiles"("Id") ON DELETE CASCADE;


--
-- Name: Follows FK_Follows_Profiles_FollowingId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Follows"
    ADD CONSTRAINT "FK_Follows_Profiles_FollowingId" FOREIGN KEY ("FollowingId") REFERENCES public."Profiles"("Id") ON DELETE CASCADE;


--
-- Name: Reviews FK_Reviews_Profiles_ProfileId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Reviews"
    ADD CONSTRAINT "FK_Reviews_Profiles_ProfileId" FOREIGN KEY ("ProfileId") REFERENCES public."Profiles"("Id") ON DELETE CASCADE;


--
-- PostgreSQL database dump complete
--

