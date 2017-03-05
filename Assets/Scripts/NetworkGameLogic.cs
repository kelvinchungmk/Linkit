﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class NetworkGameLogic : Photon.PunBehaviour
{

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	string GetGemsIDCSV( List<Gem> gems )
	{
		string csv = "";
		foreach ( Gem gem in gems )
		{
			GemNetworkInfo info = gem.GetComponent<GemNetworkInfo>();
			csv += info.ID + ",";
		}
		//csv.TrimEnd( ","[0] );
		csv = csv.Remove( csv.Length - 1 );

		return csv;
	}

	string GetGemsLaneCSV( List<Gem> gems )
	{
		string csv = "";
		foreach ( Gem gem in gems )
		{
			csv += gem.Lane + ",";
		}
		//csv.TrimEnd( ","[0] );
		csv = csv.Remove( csv.Length - 1 );

		return csv;
	}

	string[] GetGemsId( string idsCSV )
	{
		return idsCSV.Split( ","[0] );
	}

	string[] GetGemsLane( string lanesCSV )
	{
		return lanesCSV.Split( ","[0] );
	}

	public void SpawnNetworkGem( Gem gem, float spawnTime )
	{
		GemNetworkInfo info = gem.GetComponent<GemNetworkInfo>();
		photonView.RPC("SpawnNetworkGem_RPC", PhotonTargets.Others, info.ID, gem.GemType, gem.Lane, spawnTime );
	}

	[PunRPC]
	public void SpawnNetworkGem_RPC( int gemID, int gemType, int lane, float spawnTime )
	{
		GameObject.Find( "GemSpawner" ).GetComponent<GemSpawner>().SpawnNetworkGem( gemID, gemType, lane, spawnTime );
	}

	public void LinkNetworkGem( Gem gem, bool link, float linkTime )
	{
		GemNetworkInfo info = gem.GetComponent<GemNetworkInfo>();
		photonView.RPC( "LinkNetworkGem_RPC", PhotonTargets.Others, info.ID, gem.Lane, link, linkTime );
	}

	[PunRPC]
	public void LinkNetworkGem_RPC( int id, int lane, bool link, float linkTime )
	{
		GameObject.Find( "GemSpawner" ).GetComponent<GemSpawner>().LinkNetworkGem( id, lane, link, linkTime );
	}

	public void UnlinkNetworkGems( List<Gem> gems, float linkTime )
	{
		string idcsv = GetGemsIDCSV( gems );
		string lanecsv = GetGemsLaneCSV( gems );

		photonView.RPC( "UnlinkNetworkGems_RPC", PhotonTargets.Others, idcsv, lanecsv, linkTime);
	}

	[PunRPC]
	public void UnlinkNetworkGems_RPC( string idsCSV, string lanesCSV, float linkTime )
	{
		string[] ids = GetGemsId( idsCSV );
		string[] lanes = GetGemsLane( lanesCSV );
		GemSpawner spawner = GameObject.Find( "GemSpawner" ).GetComponent<GemSpawner>();

		spawner.UnlinkNetworkGems( ids, lanes, linkTime );
	}

	public void DestroyNetworkGems( List<Gem> gems, int multiplier )
	{
		string idcsv = GetGemsIDCSV( gems );
		string lanecsv = GetGemsLaneCSV( gems );

		photonView.RPC( "DestroyNetworkGems_RPC", PhotonTargets.Others, idcsv, lanecsv, multiplier );
	}

	[PunRPC]
	public void DestroyNetworkGems_RPC( string idsCSV, string lanesCSV, int multiplier )
	{
		string[] ids = GetGemsId( idsCSV );
		string[] lanes = GetGemsLane( lanesCSV );
		GemSpawner spawner = GameObject.Find( "GemSpawner" ).GetComponent<GemSpawner>();

		spawner.DestroyNetworkGems( ids, lanes, multiplier );
	}

	// Both ways
	public void CreateRepel( Gem gem )
	{
		GemNetworkInfo info = gem.GetComponent<GemNetworkInfo>();
		Gem g = gem.GetComponent<Gem>();
		photonView.RPC( "CreateRepel_RPC", PhotonTargets.Others, info.ID, g.Lane );
	}

	[PunRPC]
	public void CreateRepel_RPC( int id, int lane )
	{
		GameObject.Find("GemSpawner").GetComponent<GemSpawner>().CreateNetworkRepel( id, lane );
	}

	// OnPhotonDisconnect (Go to score)
	public override void OnPhotonPlayerDisconnected( PhotonPlayer otherPlayer )
	{
		GemSpawner spawner = GameObject.Find( "GemSpawner" ).GetComponent<GemSpawner>();

		spawner.OnNetworkDisconnect();
	}
}
