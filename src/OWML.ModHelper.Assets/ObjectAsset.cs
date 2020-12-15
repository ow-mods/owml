using UnityEngine;

namespace OWML.ModHelper.Assets
{
	public class ObjectAsset : ModAsset<GameObject>
	{
		private MeshRenderer _meshRenderer;
		private MeshFilter _meshFilter;

		public void SetMeshRenderer(MeshRenderer meshRenderer)
		{
			_meshRenderer = meshRenderer;
			SetAssetIfComplete();
		}

		public void SetMeshFilter(MeshFilter meshFilter)
		{
			_meshFilter = meshFilter;
			SetAssetIfComplete();
		}

		private void SetAssetIfComplete()
		{
			if (_meshFilter != null && _meshRenderer != null)
			{
				SetAsset(gameObject);
			}
		}
	}
}
